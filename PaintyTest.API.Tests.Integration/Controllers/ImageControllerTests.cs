using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PaintyTest.API.Contracts.Requests.User;
using PaintyTest.Data.Dto;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace PaintyTest.API.Tests.Integration.Controllers;

public class ImageControllerTests
{
    private readonly AppFactory _factory;

    public ImageControllerTests()
    {
        _factory = new AppFactory();
    }

    [Fact]
    public async Task UploadImages_ReturnsListOfImageInfo()
    {
        var user = Fakers.UserFaker.Generate();

        var client = _factory.CreateClient();

        var userCreateResponse = await client.PostAsJsonAsync("api/User/createuser", new CreateUserRequest
        {
            Username = user.Username,
            Password = user.Password,
        });

        user.Id = (await userCreateResponse.Content.ReadFromJsonAsync<UserDto>())!.Id;

        client.Auth(user.Username, user.Password);

        var images = new MultipartFormDataContent();
        for (var i = 1; i <= 3; i++)
        {
            var file = File.OpenRead($".\\TestImages\\{i}.jpg");
            var stream = new StreamContent(file);
            stream.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            images.Add(stream, "images", $"{i}.jpg");
        }

        var response = await client.PostAsync("api/Image/upload", images);
        var imageDtos = await response.Content.ReadFromJsonAsync<List<ImageDto>>();


        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        for (var i = 1; i <= 3; i++)
        {
            var dto = imageDtos[i - 1];
            Assert.Equivalent(user.Id, dto.UserId);
            Assert.Equivalent($"{i}.jpg", dto.FileName);
        }
    }

    [Fact]
    public async Task GetImageInfo_ReturnsImageInfo_WhenImageExists()
    {
        var user = Fakers.UserFaker.Generate();

        var client = _factory.CreateClient();

        _ = await client.PostAsJsonAsync("api/User/createuser", new CreateUserRequest
        {
            Username = user.Username,
            Password = user.Password,
        });

        client.Auth(user.Username, user.Password);

        var images = new MultipartFormDataContent();
        var file = File.OpenRead(".\\TestImages\\1.jpg");
        var stream = new StreamContent(file);
        stream.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        images.Add(stream, "images", "1.jpg");
        var imagesResponse = await client.PostAsync("api/Image/upload", images);
        var imageDto = (await imagesResponse.Content.ReadFromJsonAsync<List<ImageDto>>())![0];

        var response = await client.GetAsync($"api/Image/info?id={imageDto.Id}");
        var imageInfo = await response.Content.ReadFromJsonAsync<ImageDto>();

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(imageDto, imageInfo);
    }

    [Fact]
    public async Task GetImageInfo_ReturnsBadRequest_WhenImageNotExists()
    {
        var user = Fakers.UserFaker.Generate();

        var client = _factory.CreateClient();

        _ = await client.PostAsJsonAsync("api/User/createuser", new CreateUserRequest
        {
            Username = user.Username,
            Password = user.Password,
        });

        client.Auth(user.Username, user.Password);

        var response = await client.GetAsync($"api/Image/info?id=NotExists");
        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetImageInfo_ReturnsImageInfo_WhenUserFriendOfImageOwner()
    {
        var user = Fakers.UserFaker.Generate();
        var otherUser = Fakers.UserFaker.Generate();

        var client = _factory.CreateClient();

        _ = await client.PostAsJsonAsync("api/User/createuser", new CreateUserRequest
        {
            Username = user.Username,
            Password = user.Password,
        });

        _ = await client.PostAsJsonAsync("api/User/createuser", new CreateUserRequest
        {
            Username = otherUser.Username,
            Password = otherUser.Password,
        });

        client.Auth(otherUser.Username, otherUser.Password);

        _ = await client.PostAsJsonAsync("api/User/friend", new FriendWithUserRequest
        {
            Username = user.Username,
        });

        var images = new MultipartFormDataContent();
        var file = File.OpenRead(".\\TestImages\\1.jpg");
        var stream = new StreamContent(file);
        stream.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        images.Add(stream, "images", "1.jpg");
        var imagesResponse = await client.PostAsync("api/Image/upload", images);
        var imageDto = (await imagesResponse.Content.ReadFromJsonAsync<List<ImageDto>>())![0];

        client.Auth(user.Username, user.Password);
        var response = await client.GetAsync($"api/Image/info?id={imageDto.Id}");
        var imageInfo = await response.Content.ReadFromJsonAsync<ImageDto>();

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(imageDto, imageInfo);
    }

    [Fact]
    public async Task GetImageInfo_ReturnsForbidden_WhenUserNotFriendOfImageOwner()
    {
        var user = Fakers.UserFaker.Generate();
        var otherUser = Fakers.UserFaker.Generate();

        var client = _factory.CreateClient();

        _ = await client.PostAsJsonAsync("api/User/createuser", new CreateUserRequest
        {
            Username = user.Username,
            Password = user.Password,
        });

        _ = await client.PostAsJsonAsync("api/User/createuser", new CreateUserRequest
        {
            Username = otherUser.Username,
            Password = otherUser.Password,
        });

        client.Auth(otherUser.Username, otherUser.Password);

        var images = new MultipartFormDataContent();
        var file = File.OpenRead(".\\TestImages\\1.jpg");
        var stream = new StreamContent(file);
        stream.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        images.Add(stream, "images", "1.jpg");
        var imagesResponse = await client.PostAsync("api/Image/upload", images);
        var imageDto = (await imagesResponse.Content.ReadFromJsonAsync<List<ImageDto>>())![0];

        client.Auth(user.Username, user.Password);
        var response = await client.GetAsync($"api/Image/info?id={imageDto.Id}");

        Assert.Equivalent(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DownloadImage_ReturnsImage_WhenImageExists()
    {
        var user = Fakers.UserFaker.Generate();

        var client = _factory.CreateClient();

        _ = await client.PostAsJsonAsync("api/User/createuser", new CreateUserRequest
        {
            Username = user.Username,
            Password = user.Password,
        });

        client.Auth(user.Username, user.Password);

        var images = new MultipartFormDataContent();
        var file = File.OpenRead(".\\TestImages\\1.jpg");
        var stream = new StreamContent(file);
        stream.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        images.Add(stream, "images", "1.jpg");
        var imagesResponse = await client.PostAsync("api/Image/upload", images);
        var imageDto = (await imagesResponse.Content.ReadFromJsonAsync<List<ImageDto>>())![0];
        stream.Dispose();

        var response = await client.GetAsync($"api/Image/download?id={imageDto.Id}");
        var imageBytes = await response.Content.ReadAsByteArrayAsync();

        var expectedBytes = await File.ReadAllBytesAsync(".\\TestImages\\1.jpg");

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedBytes, imageBytes);
    }

    [Fact]
    public async Task DownloadImage_ReturnsBadRequest_WhenImageNotExists()
    {
        var user = Fakers.UserFaker.Generate();

        var client = _factory.CreateClient();

        _ = await client.PostAsJsonAsync("api/User/createuser", new CreateUserRequest
        {
            Username = user.Username,
            Password = user.Password,
        });

        client.Auth(user.Username, user.Password);
        var response = await client.GetAsync("api/Image/download?id=NotExists");
        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DownloadImage_ReturnsImageInfo_WhenUserFriendOfImageOwner()
    {
        var user = Fakers.UserFaker.Generate();
        var otherUser = Fakers.UserFaker.Generate();

        var client = _factory.CreateClient();

        _ = await client.PostAsJsonAsync("api/User/createuser", new CreateUserRequest
        {
            Username = user.Username,
            Password = user.Password,
        });

        _ = await client.PostAsJsonAsync("api/User/createuser", new CreateUserRequest
        {
            Username = otherUser.Username,
            Password = otherUser.Password,
        });

        client.Auth(otherUser.Username, otherUser.Password);

        _ = await client.PostAsJsonAsync("api/User/friend", new FriendWithUserRequest
        {
            Username = user.Username,
        });

        var images = new MultipartFormDataContent();
        var file = File.OpenRead(".\\TestImages\\1.jpg");
        var stream = new StreamContent(file);
        stream.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        images.Add(stream, "images", "1.jpg");
        var imagesResponse = await client.PostAsync("api/Image/upload", images);
        var imageDto = (await imagesResponse.Content.ReadFromJsonAsync<List<ImageDto>>())![0];
        stream.Dispose();

        client.Auth(user.Username, user.Password);
        var response = await client.GetAsync($"api/Image/download?id={imageDto.Id}");
        var imageBytes = await response.Content.ReadAsByteArrayAsync();

        var expectedBytes = await File.ReadAllBytesAsync(".\\TestImages\\1.jpg");

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedBytes, imageBytes);
    }

    [Fact]
    public async Task DownloadImage_ReturnsForbidden_WhenUserNotFriendOfImageOwner()
    {
        var user = Fakers.UserFaker.Generate();
        var otherUser = Fakers.UserFaker.Generate();

        var client = _factory.CreateClient();

        _ = await client.PostAsJsonAsync("api/User/createuser", new CreateUserRequest
        {
            Username = user.Username,
            Password = user.Password,
        });

        _ = await client.PostAsJsonAsync("api/User/createuser", new CreateUserRequest
        {
            Username = otherUser.Username,
            Password = otherUser.Password,
        });

        client.Auth(otherUser.Username, otherUser.Password);

        var images = new MultipartFormDataContent();
        var file = File.OpenRead(".\\TestImages\\1.jpg");
        var stream = new StreamContent(file);
        stream.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        images.Add(stream, "images", "1.jpg");
        var imagesResponse = await client.PostAsync("api/Image/upload", images);
        var imageDto = (await imagesResponse.Content.ReadFromJsonAsync<List<ImageDto>>())![0];
        stream.Dispose();

        client.Auth(user.Username, user.Password);
        var response = await client.GetAsync($"api/Image/download?id={imageDto.Id}");

        Assert.Equivalent(HttpStatusCode.Forbidden, response.StatusCode);
    }


    [Theory]
    [InlineData("api/Image/info", HttpMethod.Get)]
    [InlineData("api/Image/download", HttpMethod.Get)]
    [InlineData("api/Image/delete", HttpMethod.Delete)]
    [InlineData("api/Image/upload", HttpMethod.Post)]
    public async Task Fetch_ReturnUnauthorized_WhenUserNotExists(string path, HttpMethod method)
    {
        var client = _factory.CreateClient();
        client.Auth("Not", "Exists");
        var response = await client.SendAsync(new HttpRequestMessage(new System.Net.Http.HttpMethod(method.ToString()), path));
        Assert.Equivalent(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}