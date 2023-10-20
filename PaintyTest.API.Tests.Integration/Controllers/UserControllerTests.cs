using System.Net;
using System.Net.Http.Json;
using PaintyTest.API.Contracts.Requests.User;
using PaintyTest.API.Mapper;
using PaintyTest.Data.Dto;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace PaintyTest.API.Tests.Integration.Controllers;

public class UserControllerTests
{
    private readonly AppFactory _factory;

    public UserControllerTests()
    {
        _factory = new AppFactory();
    }

    [Fact]
    public async Task CreateUser_ReturnsUser_WhenUserWithSuchUsernameNotExists()
    {
        var user = Fakers.UserFaker.Generate();

        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("api/User/createuser",
            new CreateUserRequest
            {
                Username = user.Username,
                Password = user.Password,
            }
        );
        var userResponse = await response.Content.ReadFromJsonAsync<UserDto>();

        user.Id = userResponse!.Id;

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(user.ToDto(), userResponse);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenUserWithSuchUsernameExists()
    {
        var user = Fakers.UserFaker.Generate();

        var client = _factory.CreateClient();
        await client.PostAsJsonAsync("api/User/createuser",
            new CreateUserRequest
            {
                Username = user.Username,
                Password = user.Password,
            }
        );

        var response = await client.PostAsJsonAsync("api/User/createuser",
            new CreateUserRequest
            {
                Username = user.Username,
                Password = user.Password,
            }
        );

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetUsersFriends_ReturnsListOfUsers()
    {
        var users = Fakers.UserFaker.Generate(4);

        var client = _factory.CreateClient();
        foreach (var user in users)
        {
            var response = await client.PostAsJsonAsync("api/User/createuser",
                new CreateUserRequest
                {
                    Username = user.Username,
                    Password = user.Password,
                }
            );
            var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
            user.Id = userDto!.Id;
        }

        var expectedUsers = new List<UserDto> { users[1].ToDto(), users[3].ToDto() };

        client.Auth(users[0].Username, users[0].Password);

        await client.PostAsJsonAsync("api/User/friend",
            new FriendWithUserRequest
            {
                Username = users[1].Username,
            }
        );

        await client.PostAsJsonAsync("api/User/friend",
            new FriendWithUserRequest
            {
                Username = users[3].Username,
            }
        );

        var response2 = await client.GetAsync($"api/User/getusersfriends?id={users[0].Id}");
        var userDtos = await response2.Content.ReadFromJsonAsync<List<UserDto>>();

        Assert.Equivalent(HttpStatusCode.OK, response2.StatusCode);
        Assert.Equivalent(expectedUsers, userDtos);
    }

    [Fact]
    public async Task GetUsersFriendOf_ReturnsListOfUsers()
    {
        var users = Fakers.UserFaker.Generate(4);

        var client = _factory.CreateClient();
        foreach (var user in users)
        {
            var response = await client.PostAsJsonAsync("api/User/createuser",
                new CreateUserRequest
                {
                    Username = user.Username,
                    Password = user.Password,
                }
            );
            var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
            user.Id = userDto!.Id;
        }

        var expectedUsers = new List<UserDto> { users[1].ToDto(), users[3].ToDto() };

        client.Auth(users[1].Username, users[1].Password);
        await client.PostAsJsonAsync("api/User/friend",
            new FriendWithUserRequest
            {
                Username = users[0].Username,
            }
        );
        client.Auth(users[3].Username, users[3].Password);
        await client.PostAsJsonAsync("api/User/friend",
            new FriendWithUserRequest
            {
                Username = users[0].Username,
            }
        );

        client.Auth(users[0].Username, users[0].Password);
        var response2 = await client.GetAsync($"api/User/getusersfriendof?id={users[0].Id}");
        var userDtos = await response2.Content.ReadFromJsonAsync<List<UserDto>>();

        Assert.Equivalent(HttpStatusCode.OK, response2.StatusCode);
        Assert.Equivalent(expectedUsers, userDtos);
    }

    [Fact]
    public async Task UpdateUser_ReturnsUser_WhenUserWithSuchUsernameNotExists()
    {
        var user = Fakers.UserFaker.Generate();

        var client = _factory.CreateClient();

        var userCreateResponse = await client.PostAsJsonAsync("api/User/createuser",
            new CreateUserRequest
            {
                Username = user.Username,
                Password = user.Password,
            }
        );
        user.Id = (await userCreateResponse.Content.ReadFromJsonAsync<UserDto>())!.Id;

        var updatedUser = Fakers.UserFaker.Generate();
        updatedUser.Id = user.Id;

        client.Auth(user.Username, user.Password);
        var response = await client.PutAsJsonAsync("api/User/updateuser",
            new UpdateUserRequest
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                Password = updatedUser.Password,
                Role = updatedUser.Role,
            }
        );
        var userDto = await response.Content.ReadFromJsonAsync<UserDto>();

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(updatedUser.ToDto(), userDto);
    }

    [Fact]
    public async Task UpdateUser_ReturnsBadRequest_WhenUserWithSuchUsernameExists()
    {
        var user = Fakers.UserFaker.Generate();
        var anotherUser = Fakers.UserFaker.Generate();

        var client = _factory.CreateClient();

        var userCreateResponse = await client.PostAsJsonAsync("api/User/createuser",
            new CreateUserRequest
            {
                Username = user.Username,
                Password = user.Password,
            }
        );
        user.Id = (await userCreateResponse.Content.ReadFromJsonAsync<UserDto>())!.Id;

        _ = await client.PostAsJsonAsync("api/User/createuser",
            new CreateUserRequest
            {
                Username = anotherUser.Username,
                Password = anotherUser.Password,
            }
        );

        client.Auth(user.Username, user.Password);
        var response = await client.PutAsJsonAsync("api/User/updateuser",
            new UpdateUserRequest
            {
                Id = user.Id,
                Username = anotherUser.Username,
                Password = user.Password,
                Role = user.Role,
            }
        );

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ReturnUser_WhenUserExists()
    {
        var user = Fakers.UserFaker.Generate();

        var client = _factory.CreateClient();
        var userCreateResponse = await client.PostAsJsonAsync("api/User/createuser",
            new CreateUserRequest
            {
                Username = user.Username,
                Password = user.Password,
            }
        );
        user.Id = (await userCreateResponse.Content.ReadFromJsonAsync<UserDto>())!.Id;

        client.Auth(user.Username, user.Password);
        var response = await client.DeleteAsync($"api/User/deleteuser?id={user.Id}");
        var userDto = await response.Content.ReadFromJsonAsync<UserDto>();

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(user.ToDto(), userDto);
    }

    [Theory]
    [InlineData("api/User/getusersfriends", HttpMethod.Get)]
    [InlineData("api/User/getusersfriendof", HttpMethod.Get)]
    [InlineData("api/User/getimages", HttpMethod.Get)]
    [InlineData("api/User/updateuser", HttpMethod.Put)]
    [InlineData("api/User/deleteuser", HttpMethod.Delete)]
    [InlineData("api/User/friend", HttpMethod.Post)]
    [InlineData("api/User/unfriend", HttpMethod.Put)]
    public async Task Fetch_ReturnUnauthorized_WhenUserNotExists(string path, HttpMethod method)
    {
        var client = _factory.CreateClient();
        client.Auth("Not", "Exists");
        var response = await client.SendAsync(new HttpRequestMessage(new System.Net.Http.HttpMethod(method.ToString()), path));
        Assert.Equivalent(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}