namespace PaintyTest.API.Contracts.Requests.User;

public class UnfriendWithUserRequest
{
    public Guid Id { get; set; } = Guid.Empty;

    public string? Username { get; set; } = null;
}