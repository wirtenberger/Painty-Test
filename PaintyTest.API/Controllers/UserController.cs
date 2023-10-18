using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaintyTest.Contracts.Requests.User;
using PaintyTest.Data.Entities;
using PaintyTest.Mapper;
using PaintyTest.Services;

namespace PaintyTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly FriendshipService _friendshipService;
    private readonly UserService _userService;

    private string Username => User.FindFirst("name")!.Value;

    public UserController(UserService userService, FriendshipService friendshipService)
    {
        _userService = userService;
        _friendshipService = friendshipService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("getall")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users.Select(u => u.ToDto()));
    }

    [Authorize]
    [HttpGet("getusersfriends")]
    public async Task<IActionResult> GetUsersFriends([FromQuery] GetUserRequest request)
    {
        if (request.Id != Guid.Empty)
        {
            if (!await UserHasPermission(request.Id))
            {
                return Forbid();
            }

            var users = await _userService.GetUsersFriends(request.Id);
            return Ok(users.Select(u => u.ToDto()));
        }

        if (request.Username is not null)
        {
            var user = await _userService.GetByUsername(request.Username);
            if (!await UserHasPermission(user.Id))
            {
                return Forbid();
            }

            var users = await _userService.GetUsersFriends(user.Id);
            return Ok(users.Select(u => u.ToDto()));
        }

        return BadRequest();
    }

    [Authorize]
    [HttpGet("getusersfriendof")]
    public async Task<IActionResult> GetUsersFriendOf([FromQuery] GetUserRequest request)
    {
        if (request.Id != Guid.Empty)
        {
            if (!await UserHasPermission(request.Id))
            {
                return Forbid();
            }

            var users = await _userService.GetUsersFriendOf(request.Id);
            return Ok(users.Select(u => u.ToDto()));
        }

        if (request.Username is not null)
        {
            var user = await _userService.GetByUsername(request.Username);
            if (!await UserHasPermission(user.Id))
            {
                return Forbid();
            }

            var users = await _userService.GetUsersFriendOf(user.Id);
            return Ok(users.Select(u => u.ToDto()));
        }

        return BadRequest();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("getuser")]
    public async Task<IActionResult> GetUser([FromQuery] GetUserRequest request)
    {
        if (request.Id != Guid.Empty)
        {
            var user = await _userService.GetById(request.Id);
            return Ok(user.ToDto());
        }

        if (request.Username is not null)
        {
            var user = await _userService.GetByUsername(request.Username);
            return Ok(user.ToDto());
        }

        return BadRequest();
    }

    [Authorize]
    [HttpGet("getimages")]
    public async Task<IActionResult> GetUsersImages([FromQuery] GetUserRequest request)
    {
        if (request.Id != Guid.Empty)
        {
            if (!await UserHasPermission(request.Id))
            {
                return Forbid();
            }

            var user = await _userService.GetById(request.Id);
            return Ok(user.Images.Select(u => u.ToDto()));
        }

        if (request.Username is not null)
        {
            var user = await _userService.GetByUsername(request.Username);
            if (!await UserHasPermission(user.Id))
            {
                return Forbid();
            }

            return Ok(user.Images.Select(u => u.ToDto()));
        }

        return BadRequest();
    }

    [HttpPost("createuser")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateAsync(
            request.ToUser()
        );

        return Ok(user.ToDto());
    }

    [Authorize]
    [HttpPut("updateuser")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        if (!await UserHasPermission(request.Id))
        {
            return Forbid();
        }

        if (request.Role == "Admin" && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var updatedUser = await _userService.UpdateAsync(
            request.ToUser()
        );

        return Ok(updatedUser.ToDto());
    }

    [Authorize]
    [HttpDelete("deleteuser")]
    public async Task<IActionResult> DeleteUser([FromQuery] DeleteUserRequest request)
    {
        if (!await UserHasPermission(request.Id))
        {
            return Forbid();
        }

        var deletedUser = await _userService.DeleteAsync(request.Id);

        return Ok(deletedUser.ToDto());
    }

    [Authorize]
    [HttpPost("friend")]
    public async Task<IActionResult> FriendWithUser([FromBody] FriendWithUserRequest request)
    {
        var user = await _userService.GetByUsername(Username);

        if (request.Id != Guid.Empty)
        {
            await _friendshipService.MakeFriendOf(user.Id, request.Id);
        }
        else if (request.Username is not null)
        {
            var friendUser = await _userService.GetByUsername(request.Username);
            await _friendshipService.MakeFriendOf(user.Id, friendUser.Id);
        }
        else
        {
            return BadRequest();
        }

        return Ok();
    }

    [Authorize]
    [HttpPut("unfriend")]
    public async Task<IActionResult> UnfriendWithUser([FromBody] UnfriendWithUserRequest request)
    {
        var user = await _userService.GetByUsername(Username);

        if (request.Id != Guid.Empty)
        {
            await _friendshipService.StopBeingFriendOf(user.Id, request.Id);
        }
        else if (request.Username is not null)
        {
            var friendUser = await _userService.GetByUsername(request.Username);
            await _friendshipService.StopBeingFriendOf(user.Id, friendUser.Id);
        }
        else
        {
            return BadRequest();
        }

        return Ok();
    }


    private async Task<bool> UserHasPermission(Guid requiredId)
    {
        var user = await _userService.GetByUsername(Username);
        return User.IsInRole("Admin") || user.Id == requiredId;
    }
}