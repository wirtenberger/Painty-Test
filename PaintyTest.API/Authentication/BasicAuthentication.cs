using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using PaintyTest.Services;

namespace PaintyTest.Authentication;

public static class BasicAuthentication
{
    public const string SchemeName = "Basic";
    public const string DisplayName = "BasicAuth";
}

public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var userService = Context.RequestServices.GetRequiredService<UserService>();

        if (!Context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var authHeader))
        {
            return AuthenticateResult.Fail("Authorization header not provided");
        }

        var strHeader = authHeader.ToString();
        if (!strHeader.Contains(BasicAuthentication.SchemeName))
        {
            return AuthenticateResult.Fail("Authorization header value incorrect");
        }

        strHeader = strHeader[BasicAuthentication.SchemeName.Length..];
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(strHeader));
        var split = decoded.Split(":");
        var username = split[0].Length > 0 ? split[0] : null;
        var password = split[1].Length > 0 ? split[1] : null;

        if (username is null || password is null)
        {
            Context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return await Task.FromResult(AuthenticateResult.Fail("Authorization header value incorrect"));
        }

        if (!await userService.IsAuthenticated(username, password))
        {
            Context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return await Task.FromResult(AuthenticateResult.Fail("Failed to authorize"));
        }

        var user = await userService.GetByUsername(username);

        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            new Claim[]
            {
                new("name", username),
                new(ClaimTypes.Role, user.Role),
            }
        ));
        return AuthenticateResult.Success(new AuthenticationTicket(principal, BasicAuthentication.SchemeName));
    }
}

public class BasicAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
}

public static class BasicAuthenticationExtension
{
    public static void AddBasicAuth(this AuthenticationBuilder builder, Action<BasicAuthenticationSchemeOptions>? configureOptions = null)
    {
        builder.AddScheme<BasicAuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthentication.SchemeName, BasicAuthentication.DisplayName, configureOptions);

        builder.Services.AddAuthorization(opts =>
        {
            opts.AddPolicy(BasicAuthentication.SchemeName, opts =>
            {
                opts.AddAuthenticationSchemes(BasicAuthentication.SchemeName);
                opts.RequireClaim("name");
                opts.RequireClaim(ClaimTypes.Role);
            });

            opts.DefaultPolicy = opts.GetPolicy(BasicAuthentication.SchemeName)!;
        });
    }
}