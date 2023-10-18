using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaintyTest.Data.Entities;

namespace PaintyTest.Data.EntityConfigurations;

public class UsersUsersEntityConfiguration : IEntityTypeConfiguration<UsersUsers>
{
    public void Configure(EntityTypeBuilder<UsersUsers> builder)
    {
        builder.Property(u => u.Id).ValueGeneratedOnAdd();
    }
}