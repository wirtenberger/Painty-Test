using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PaintyTest;
using PaintyTest.Authentication;
using PaintyTest.Contracts.Repositories;
using PaintyTest.Data;
using PaintyTest.Middlewares;
using PaintyTest.Repositories;
using PaintyTest.Services;

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

app.UseMiddleware<ExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var applicationDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
if (applicationDbContext.Database.GetPendingMigrations().Any())
{
    applicationDbContext.Database.Migrate();
}

app.Run();