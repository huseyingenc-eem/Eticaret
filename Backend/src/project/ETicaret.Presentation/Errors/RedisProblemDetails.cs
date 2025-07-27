using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ETicaret.Presentation.Errors;

/// <summary>
/// Redis hataları için HTTP yanıtında kullanılacak ProblemDetails yapısı.
/// </summary>
public class RedisProblemDetails : ProblemDetails
{
    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; set; }

    [JsonPropertyName("userFriendlyMessage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? UserFriendlyMessage { get; set; }

    [JsonPropertyName("redisDetails")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? RedisDetails { get; set; }

    public RedisProblemDetails(
        string detail,
        string errorCode,
        string? userFriendlyMessage = null,
        object? redisDetails = null)
    {
        Title = "Redis Servisi Hatası";
        Detail = detail;
        Status = StatusCodes.Status503ServiceUnavailable;
        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4";
        ErrorCode = errorCode;
        UserFriendlyMessage = userFriendlyMessage ?? "Önbellek servisi geçici olarak kullanılamıyor.";
        RedisDetails = redisDetails;
    }
}