using System.ComponentModel.DataAnnotations;

namespace PaintyTest.Contracts.Requests.User;

public class FriendWithUserRequest
{
    public Guid Id { get; set; } = Guid.Empty;

    public string? Username { get; set; } = null;
}