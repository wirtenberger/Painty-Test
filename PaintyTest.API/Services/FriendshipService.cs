using PaintyTest.Contracts.Exceptions;
using PaintyTest.Contracts.Repositories;
using PaintyTest.Data.Entities;

namespace PaintyTest.Services;

public class FriendshipService
{
    private readonly IUserRepository _userRepository;
    private readonly IUsersUsersRepository _usersUsersRepository;

    public FriendshipService(IUserRepository userRepository,
                             IUsersUsersRepository usersUsersRepository)
    {
        _userRepository = userRepository;
        _usersUsersRepository = usersUsersRepository;
    }

    public async Task MakeFriendOf(Guid userId, Guid newFriendId)
    {
        if (await IsUserFriendOf(userId, newFriendId))
        {
            return;
        }

        if (await _userRepository.GetByIdAsync(userId) is null)
        {
            throw new EntityNotFoundException(typeof(User), userId.ToString());
        }

        if (await _userRepository.GetByIdAsync(newFriendId) is null)
        {
            throw new EntityNotFoundException(typeof(User), userId.ToString());
        }

        await _usersUsersRepository.CreateAsync(new UsersUsers
        {
            UserId = userId,
            UsersFriendId = newFriendId,
        });
    }

    public async Task StopBeingFriendOf(Guid userId, Guid oldFriendId)
    {
        if (await _userRepository.GetByIdAsync(userId) is null)
        {
            throw new EntityNotFoundException(typeof(User), userId.ToString());
        }

        if (await _userRepository.GetByIdAsync(oldFriendId) is null)
        {
            throw new EntityNotFoundException(typeof(User), userId.ToString());
        }

        var uu = (await _usersUsersRepository.GetAllAsync(
            u => u.UserId == userId && u.UsersFriendId == oldFriendId
        )).SingleOrDefault();

        if (uu is not null)
        {
            await _usersUsersRepository.DeleteAsync(uu.Id);
        }
    }

    public async Task<bool> IsUserFriendOf(Guid userId, Guid possibleFriendId)
    {
        var ufus = await _usersUsersRepository.GetAllAsync(
            ufu => ufu.UserId == userId && ufu.UsersFriendId == possibleFriendId
        );
        return ufus.Any();
    }
}