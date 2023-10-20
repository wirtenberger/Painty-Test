using System.ComponentModel.DataAnnotations;

namespace PaintyTest.API.Contracts.Requests.Image;

public class GetImageRequest
{
    [Required]
    public Guid Id { get; set; }
}