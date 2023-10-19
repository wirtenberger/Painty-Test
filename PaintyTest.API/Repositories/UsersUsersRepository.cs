using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PaintyTest.API.Contracts.Repositories;
using PaintyTest.Data;
using PaintyTest.Data.Entities;

namespace PaintyTest.API.Repositories;

public class UsersUsersRepository : IUsersUsersRepository
{
    private readonly AppDbContext _appDbContext;

    private DbSet<UsersUsers> FriendSet => _appDbContext.FriendUsers;

    public UsersUsersRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }


    public Task<List<UsersUsers>> GetAllAsync(Expression<Func<UsersUsers, bool>>? filter)
    {
        if (filter == null)
        {
            return FriendSet.ToListAsync();
        }

        return FriendSet.Where(filter).ToListAsync();
    }

    public async Task<UsersUsers?> GetByIdAsync(int id)
    {
        return await FriendSet.FindAsync(id);
    }

    public async Task<UsersUsers> CreateAsync(UsersUsers usersUsers)
    {
        return (await FriendSet.AddAsync(usersUsers)).Entity;
    }

    public async Task<UsersUsers> DeleteAsync(int id)
    {
        var ufu = (await GetByIdAsync(id))!;
        return FriendSet.Remove(ufu).Entity;
    }
}