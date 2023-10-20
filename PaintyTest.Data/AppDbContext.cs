using Microsoft.EntityFrameworkCore;
using PaintyTest.Data.Entities;
using PaintyTest.Data.EntityConfigurations;

namespace PaintyTest.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Image> Images { get; set; } = default!;
    public DbSet<UsersUsers> FriendUsers { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UsersUsersEntityConfiguration());
    }
}