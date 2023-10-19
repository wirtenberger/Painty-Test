using PaintyTest.API.Contracts.Requests.Image;
using PaintyTest.Data.Dto;
using PaintyTest.Data.Entities;

namespace PaintyTest.API.Mapper;

public static class ImageMapper
{
    public static Image ToImage(this UpdateImageRequest request)
    {
        return new Image
        {
            Id = request.Id,
            FileName = request.FileName,
            RelativePath = request.RelativePath,
            UserId = request.UserId,
        };
    }

    public static ImageDto ToDto(this Image image)
    {
        return new ImageDto
        {
            Id = image.Id,
            FileName = image.FileName,
            UserId = image.UserId,
        };
    }
}