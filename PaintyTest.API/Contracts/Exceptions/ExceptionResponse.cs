using System.Text.Json.Serialization;

namespace PaintyTest.Contracts.Exceptions;

public class ExceptionResponse
{
    public int StatusCode { get; set; }

    [JsonPropertyName("description")]
    public List<string> Errors { get; set; } = new();
}