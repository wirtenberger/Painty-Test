using System.ComponentModel.DataAnnotations;

namespace PaintyTest.API.Contracts.Requests.Image;

public class UpdateImageRequest
{
    [Required]
    public Guid Id { get; set; } = default!;

    [Required]
    public string FileName { get; set; } = default!;

    [Required]
    public string RelativePath { get; set; } = default!;

    [Required]
    public Guid UserId { get; set; } = default!;
}