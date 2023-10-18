using System.ComponentModel.DataAnnotations.Schema;

namespace PaintyTest.Data.Entities;

public class User
{
    private List<Image> _images = new();

    public Guid Id { get; set; } = default!;

    public string Username { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string Role { get; set; } = "User";

    public IReadOnlyCollection<Image> Images => _images.AsReadOnly();

    [InverseProperty(nameof(FriendOf))]
    public List<User> Friends { get; set; } = new();

    [InverseProperty(nameof(Friends))]
    public List<User> FriendOf { get; set; } = new();
}