using System.ComponentModel.DataAnnotations;

namespace PaintyTest.API.Contracts.Requests.Image;

public class DeleteImageRequest
{
    [Required]
    public Guid Id { get; set; }
}