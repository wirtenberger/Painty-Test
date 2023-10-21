using System.ComponentModel.DataAnnotations;

namespace PaintyTest.API.Contracts.Requests.User;

public class DeleteUserRequest
{
    [Required]
    public Guid Id { get; set; } = default!;
}