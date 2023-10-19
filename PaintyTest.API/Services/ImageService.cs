using System.Linq.Expressions;
using PaintyTest.API.Contracts.Exceptions;
using PaintyTest.API.Contracts.Repositories;
using PaintyTest.Data.Entities;

namespace PaintyTest.API.Services;

public class ImageService
{
    private readonly IImageRepository _imageRepository;

    private readonly ILogger<ImageService> _logger;

    public ImageService(IImageRepository imageRepository, ILogger<ImageService> logger)
    {
        _imageRepository = imageRepository;
        _logger = logger;
    }

    public Task<List<Image>> GetAllImagesAsync(Expression<Func<Image, bool>>? filter = null)
    {
        return _imageRepository.GetAllAsync(filter);
    }

    public async Task<Image> UpdateImageAsync(Image image)
    {
        var existingImage = await _imageRepository.GetByIdAsync(image.Id);
        if (existingImage is null)
        {
            throw new EntityNotFoundException(typeof(Image), image.Id.ToString());
        }

        return await _imageRepository.UpdateAsync(image);
    }

    public async Task<List<Image>> UploadImagesAsync(User user, FormFileCollection images)
    {
        var createdFiles = new List<Image>();
        try
        {
            foreach (var imageFormFile in images)
            {
                var newFileName = $"{Guid.NewGuid()}-{imageFormFile.FileName}";
                await using var file = ImageStorage.CreateImageFile(newFileName, user.Id.ToString());
                await imageFormFile.CopyToAsync(file);

                var image = new Image
                {
                    Id = Guid.NewGuid(),
                    FileName = imageFormFile.FileName,
                    RelativePath = Path.Join(user.Id.ToString(), newFileName),
                    UserId = user.Id,
                };

                createdFiles.Add(image);
                await _imageRepository.CreateAsync(image);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);

            foreach (var createdFile in createdFiles)
            {
                ImageStorage.DeleteImageFile(createdFile.RelativePath);
            }

            throw;
        }

        return createdFiles;
    }

    public async Task<Image> GetByIdAsync(Guid id)
    {
        var image = await _imageRepository.GetByIdAsync(id);
        if (image is null)
        {
            throw new EntityNotFoundException(typeof(Image), id.ToString());
        }

        return image;
    }

    public async Task<Image> DeleteImageAsync(Guid imageId)
    {
        var image = await GetByIdAsync(imageId);

        await _imageRepository.DeleteAsync(image.Id);
        ImageStorage.DeleteImageFile(image.RelativePath);

        return image;
    }
}