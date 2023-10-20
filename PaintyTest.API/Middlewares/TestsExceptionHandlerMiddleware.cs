using PaintyTest.API.Contracts.Exceptions;
using PaintyTest.Data;

namespace PaintyTest.API.Middlewares;

public class TestsExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public TestsExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        try
        {
            await _next(context);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            if (ex is BaseApiException be)
            {
                context.Response.StatusCode = be.StatusCode;
            }
            else
            {
                context.Response.StatusCode = 500;
            }
        }
    }
}