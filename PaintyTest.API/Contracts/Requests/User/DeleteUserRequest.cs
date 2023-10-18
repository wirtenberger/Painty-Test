using System.ComponentModel.DataAnnotations;

namespace PaintyTest.Contracts.Requests.User;

public class DeleteUserRequest
{
    [Required]
    public Guid Id { get; set; } = default!;
}