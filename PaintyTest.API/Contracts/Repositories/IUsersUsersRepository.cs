using System.Linq.Expressions;
using PaintyTest.Data.Entities;

namespace PaintyTest.Contracts.Repositories;

public interface IUsersUsersRepository
{
    Task<List<UsersUsers>> GetAllAsync(Expression<Func<UsersUsers, bool>>? filter);
    Task<UsersUsers?> GetByIdAsync(int id);
    Task<UsersUsers> CreateAsync(UsersUsers usersUsers);
    Task<UsersUsers> DeleteAsync(int id);
}