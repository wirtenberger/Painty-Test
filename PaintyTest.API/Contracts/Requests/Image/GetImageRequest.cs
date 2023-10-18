using System.ComponentModel.DataAnnotations;

namespace PaintyTest.Contracts.Requests.Image;

public class GetImageRequest
{
    [Required]
    public Guid Id { get; set; }
}