using PaintyTest.Contracts.Requests.User;
using PaintyTest.Data.Dto;
using PaintyTest.Data.Entities;

namespace PaintyTest.Mapper;

public static class UserMapper
{
    public static User ToUser(this CreateUserRequest request)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Password = request.Password,
        };
    }

    public static User ToUser(this UpdateUserRequest request)
    {
        return new User
        {
            Id = request.Id,
            Username = request.Username,
            Password = request.Password,
        };
    }

    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
        };
    }
}