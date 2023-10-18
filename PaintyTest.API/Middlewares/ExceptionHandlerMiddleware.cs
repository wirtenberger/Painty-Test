using PaintyTest.Contracts.Exceptions;
using PaintyTest.Data;

namespace PaintyTest.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext, ILogger<ExceptionHandlerMiddleware> logger)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await _next(context);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            await transaction.RollbackAsync();
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