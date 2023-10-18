using System.ComponentModel.DataAnnotations;

namespace PaintyTest.Contracts.Requests.User;

public class CreateUserRequest
{
    [Required]
    public string Username { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}