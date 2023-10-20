using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PaintyTest.API.Contracts.Repositories;
using PaintyTest.Data;
using PaintyTest.Data.Entities;

namespace PaintyTest.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _appDbContext;

    private DbSet<User> UsersSet => _appDbContext.Users;


    public UserRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public Task<List<User>> GetAllAsync(Expression<Func<User, bool>>? filter)
    {
        if (filter == null)
        {
            return UsersSet.ToListAsync();
        }

        return UsersSet.Where(filter).ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await UsersSet.FindAsync(id);
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return UsersSet.SingleOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> CreateAsync(User user)
    {
        return (await UsersSet.AddAsync(user)).Entity;
    }

    public Task<User> UpdateAsync(User user)
    {
        return Task.FromResult(UsersSet.Update(user).Entity);
    }

    public async Task<User> DeleteAsync(Guid id)
    {
        var user = (await GetByIdAsync(id))!;
        return UsersSet.Remove(user).Entity;
    }
}