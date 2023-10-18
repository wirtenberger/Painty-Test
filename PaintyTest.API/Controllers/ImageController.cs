using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaintyTest.Contracts.Requests.Image;
using PaintyTest.Mapper;
using PaintyTest.Services;

namespace PaintyTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly FriendshipService _friendshipService;

    private readonly ImageService _imageService;
    private readonly UserService _userService;
    private string Username => User.FindFirst("name")!.Value;

    public ImageController(ImageService imageService, UserService userService, FriendshipService friendshipService)
    {
        _imageService = imageService;
        _userService = userService;
        _friendshipService = friendshipService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("getall")]
    public async Task<IActionResult> GetAllImages()
    {
        var images = await _imageService.GetAllImagesAsync();
        return Ok(images.Select(i => i.ToDto()));
    }

    [Authorize]
    [HttpGet("info")]
    public async Task<IActionResult> GetImageInfo([FromQuery] GetImageRequest request)
    {
        var image = await _imageService.GetByIdAsync(request.Id);

        var user = await _userService.GetByUsername(Username);

        if (await UserHasPermission(image.UserId) || await _friendshipService.IsUserFriendOf(image.UserId, user.Id))
        {
            return Ok(image.ToDto());
        }

        return Forbid();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateImage([FromBody] UpdateImageRequest request)
    {
        var image = await _imageService.UpdateImageAsync(request.ToImage());

        return Ok(image.ToDto());
    }

    [Authorize]
    [HttpGet("download")]
    public async Task<IActionResult> DownloadImage([FromQuery] GetImageRequest request)
    {
        var image = await _imageService.GetByIdAsync(request.Id);
        var user = await _userService.GetByUsername(Username);

        if (user.Id != image.UserId && !await _friendshipService.IsUserFriendOf(image.UserId, user.Id))
        {
            return Forbid();
        }

        var file = ImageStorage.GetImageFile(image.RelativePath);

        return File(file, DetermineContentType(image.RelativePath), image.FileName);
    }


    [Authorize]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteImage([FromQuery] DeleteImageRequest request)
    {
        var image = await _imageService.GetByIdAsync(request.Id);

        if (!await UserHasPermission(image.UserId))
        {
            return Forbid();
        }

        var deleted = await _imageService.DeleteImageAsync(image.Id);

        return Ok(deleted.ToDto());
    }

    [Authorize]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadImages([FromForm] UploadImagesRequest request)
    {
        var user = await _userService.GetByUsername(Username);

        var images = await _imageService.UploadImagesAsync(user, request.Images);

        return Ok(
            images.Select(i => i.ToDto())
        );
    }

    private async Task<bool> UserHasPermission(Guid requiredId)
    {
        var user = await _userService.GetByUsername(Username);
        return User.IsInRole("Admin") || user.Id == requiredId;
    }

    private string DetermineContentType(string imagePath)
    {
        var extension = Path.GetExtension(Path.Join(ImageStorage.ImageStorageDirectoryPath, imagePath));
        switch (extension)
        {
            case ".jpg" or ".jpeg":
                return "image/jpeg";
            case ".webp":
                return "image/webp";
            case ".png":
                return "image/png";
            default:
                return "application/unkown";
        }
    }
}