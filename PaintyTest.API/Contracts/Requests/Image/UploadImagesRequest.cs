using System.ComponentModel.DataAnnotations;

namespace PaintyTest.API.Contracts.Requests.Image;

public class UploadImagesRequest
{
    [Required]
    public FormFileCollection Images { get; set; } = default!;
}