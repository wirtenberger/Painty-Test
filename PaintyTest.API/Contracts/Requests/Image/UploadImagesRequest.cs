using System.ComponentModel.DataAnnotations;

namespace PaintyTest.Contracts.Requests.Image;

public class UploadImagesRequest
{
    [Required]
    public FormFileCollection Images { get; set; } = default!;
}