using System.Linq.Expressions;
using PaintyTest.Data.Entities;

namespace PaintyTest.Contracts.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync(Expression<Func<User, bool>>? filter);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<User> DeleteAsync(Guid id);
}