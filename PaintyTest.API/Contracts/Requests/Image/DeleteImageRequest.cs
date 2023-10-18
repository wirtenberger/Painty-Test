using System.ComponentModel.DataAnnotations;

namespace PaintyTest.Contracts.Requests.Image;

public class DeleteImageRequest
{
    [Required]
    public Guid Id { get; set; }
}