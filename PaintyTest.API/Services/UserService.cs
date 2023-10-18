using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using PaintyTest.Contracts.Exceptions;
using PaintyTest.Contracts.Repositories;
using PaintyTest.Data.Entities;

namespace PaintyTest.Services;

public class UserService
{
    private readonly PasswordHasher<User> _passwordHasher = new();
    private readonly IUsersUsersRepository _userFriendRepository;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IUsersUsersRepository userFriendRepository)
    {
        _userRepository = userRepository;
        _userFriendRepository = userFriendRepository;
    }

    public Task<List<User>> GetAllAsync(Expression<Func<User, bool>>? filter = null)
    {
        return _userRepository.GetAllAsync(filter);
    }

    public async Task<User> GetById(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            throw new EntityNotFoundException(typeof(User), id.ToString());
        }

        return user;
    }

    public async Task<User> GetByUsername(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user is null)
        {
            throw new EntityNotFoundException(typeof(User), username);
        }

        return user;
    }

    public async Task<User> CreateAsync(User user)
    {
        if (await _userRepository.GetByUsernameAsync(user.Username) is not null)
        {
            throw new EntityExistsException(typeof(User), nameof(User.Username), user.Username);
        }

        user.Password = _passwordHasher.HashPassword(user, user.Password);
        return await _userRepository.CreateAsync(user);
    }

    public async Task<User> UpdateAsync(User user)
    {
        if (await _userRepository.GetByIdAsync(user.Id) is null)
        {
            throw new EntityNotFoundException(typeof(User), user.Id.ToString());
        }

        var existing = await _userRepository.GetByUsernameAsync(user.Username);
        if (existing is not null && existing.Username == user.Username && existing.Id != user.Id)
        {
            throw new EntityExistsException(typeof(User), nameof(User.Username), user.Username);
        }

        user.Password = _passwordHasher.HashPassword(user, user.Password);

        return await _userRepository.UpdateAsync(user);
    }

    public async Task<User> DeleteAsync(Guid id)
    {
        if (await _userRepository.GetByIdAsync(id) is null)
        {
            throw new EntityNotFoundException(typeof(User), id.ToString());
        }

        return await _userRepository.DeleteAsync(id);
    }

    public async Task<List<User>> GetUsersFriends(Guid userId)
    {
        var uus = await _userFriendRepository.GetAllAsync(
            ufu => ufu.UserId == userId
        );

        List<User> users = new();
        foreach (var uu in uus)
        {
            users.Add((await _userRepository.GetByIdAsync(uu.UsersFriendId))!);
        }

        return users;
    }

    public async Task<List<User>> GetUsersFriendOf(Guid userId)
    {
        var uus = await _userFriendRepository.GetAllAsync(
            ufu => ufu.UsersFriendId == userId
        );

        List<User> users = new();
        foreach (var uu in uus)
        {
            users.Add((await _userRepository.GetByIdAsync(uu.UserId))!);
        }

        return users;
    }

    public async Task<bool> IsAuthenticated(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user is null)
        {
            return false;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
        return result == PasswordVerificationResult.Success
               || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}