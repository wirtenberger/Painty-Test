namespace PaintyTest.Data.Dto;

public class UserDto
{
    public Guid Id { get; set; } = default!;

    public string Username { get; set; } = default!;

    public string Role { get; set; } = default!;
}