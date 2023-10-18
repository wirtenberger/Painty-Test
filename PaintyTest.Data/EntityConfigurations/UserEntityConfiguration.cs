using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaintyTest.Data.Entities;

namespace PaintyTest.Data.EntityConfigurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasIndex(u => u.Username)
            .IsUnique();

        builder
            .HasMany(u => u.Friends)
            .WithMany(u => u.FriendOf)
            .UsingEntity<UsersUsers>();

        builder
            .HasMany(u => u.FriendOf)
            .WithMany(u => u.Friends)
            .UsingEntity<UsersUsers>();


        builder
            .HasMany(u => u.Images)
            .WithOne(i => i.User)
            .HasForeignKey(i => i.UserId)
            .Metadata.PrincipalToDependent!.SetPropertyAccessMode(PropertyAccessMode.Field);


        builder.Navigation(u => u.Images).AutoInclude();
    }
}