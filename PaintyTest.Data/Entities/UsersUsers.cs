using System.ComponentModel.DataAnnotations.Schema;

namespace PaintyTest.Data.Entities;

public class UsersUsers
{
    public int Id { get; set; } = default!;

    public Guid UserId { get; set; } = default!;

    public Guid UsersFriendId { get; set; } = default!;
}