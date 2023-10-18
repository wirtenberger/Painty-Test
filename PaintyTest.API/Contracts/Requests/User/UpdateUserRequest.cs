using System.ComponentModel.DataAnnotations;

namespace PaintyTest.Contracts.Requests.User;

public class UpdateUserRequest
{
    [Required]
    public Guid Id { get; set; } = default!;

    [Required]
    public string Username { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;

    [Required]
    public string Role { get; set; } = default!;
}