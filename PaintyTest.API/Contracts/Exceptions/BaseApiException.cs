namespace PaintyTest.Contracts.Exceptions;

public class BaseApiException : Exception
{
    public const int DefaultCode = StatusCodes.Status500InternalServerError;

    protected List<string> Errors { get; set; }

    public int StatusCode { get; set; }

    protected BaseApiException(int statusCode, string description) : this(
        statusCode,
        new List<string> { description }
    )
    {
    }

    public BaseApiException(Exception e) : this(DefaultCode, e.Message)
    {
    }

    public BaseApiException(int statusCode, List<string> description)
    {
        StatusCode = statusCode;
        Errors = description;
    }
}