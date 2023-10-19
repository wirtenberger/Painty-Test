using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PaintyTest.API;
using PaintyTest.API.Authentication;
using PaintyTest.API.Contracts.Repositories;
using PaintyTest.API.Middlewares;
using PaintyTest.API.Repositories;
using PaintyTest.API.Services;
using PaintyTest.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.LoadAppConfig();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Basic Auth",
        In = ParameterLocation.Header,
        Scheme = BasicAuthentication.SchemeName,
        Type = SecuritySchemeType.Http,
    });

    opts.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Basic" },
            },
            new List<string>()
        },
    });
});

builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    opts.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = BasicAuthentication.SchemeName;
    options.DefaultScheme = BasicAuthentication.SchemeName;
}).AddBasicAuth();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IUsersUsersRepository, UsersUsersRepository>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<FriendshipService>();

var app = builder.Build();

if (app.Environment.IsEnvironment("Test"))
{
    app.UseMiddleware<TestsExceptionHandlerMiddleware>();
}
else
{
    app.UseMiddleware<ExceptionHandlerMiddleware>();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var applicationDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (applicationDbContext.Database.GetPendingMigrations().Any())
        {
            await applicationDbContext.Database.MigrateAsync();
        }
    }
    catch
    {
        scope.ServiceProvider.GetRequiredService<ILogger<PaintyTest.API.Program>>().LogError("Apply migrations manually");
    }
}

app.Run();

namespace PaintyTest.API
{
    public partial class Program
    {
    }
}