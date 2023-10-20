using Bogus;
using PaintyTest.Data.Entities;

namespace PaintyTest.API.Tests.Integration;

public static class Fakers
{
    public static Faker<User> UserFaker { get; } = new Faker<User>()
                                                   .RuleFor(u => u.Id, _ => Guid.NewGuid())
                                                   .RuleFor(u => u.Username, f => f.Name.FirstName())
                                                   .RuleFor(u => u.Password, f => f.Random.AlphaNumeric(8))
                                                   .RuleFor(u => u.Role, _ => "User");
}