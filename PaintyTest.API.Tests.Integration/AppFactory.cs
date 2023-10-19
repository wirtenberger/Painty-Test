using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaintyTest.API.Authentication;
using PaintyTest.Data;

namespace PaintyTest.API.Tests.Integration;

public class AppFactory : WebApplicationFactory<Program>
{
    private readonly Action<IServiceCollection>? _serviceOverride;

    public AppFactory(Action<IServiceCollection>? serviceOverride = null)
    {
        _serviceOverride = serviceOverride;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        if (_serviceOverride is not null)
        {
            builder.ConfigureServices(_serviceOverride);
        }

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<AppDbContext>));

            services.Remove(dbContextDescriptor!);

            services.AddDbContext<AppDbContext>(opts =>
            {
                opts
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseInMemoryDatabase("db");
            });
        });
        builder.UseEnvironment("Test");
    }
}

public static class HttpClientAuthExtension
{
    public static void Auth(this HttpClient client, string username, string password)
    {
        var bytes = Encoding.UTF8.GetBytes($"{username}:{password}");
        var authHeaderValue = Convert.ToBase64String(bytes);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthentication.SchemeName, authHeaderValue);
    }
}