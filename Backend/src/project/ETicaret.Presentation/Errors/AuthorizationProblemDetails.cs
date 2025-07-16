using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ETicaret.Presentation.Errors;

public class AuthorizationProblemDetails : ProblemDetails
{
    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; set; }

    [JsonPropertyName("userFriendlyMessage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? UserFriendlyMessage { get; set; }

    [JsonPropertyName("additionalData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? AdditionalData { get; set; }

    public AuthorizationProblemDetails(string detail, string errorCode, string? userFriendlyMessage = null, object? additionalData = null, int statusCode = StatusCodes.Status401Unauthorized)
    {
        Title = "Yetkilendirme Hatası";
        Detail = detail;
        Status = statusCode; // 401 veya 403 olabilir
        Type = statusCode == StatusCodes.Status401Unauthorized ? "urn:ietf:rfc:7235#section-3.1" : "urn:ietf:rfc:7231#section-6.5.3";
        ErrorCode = errorCode;
        UserFriendlyMessage = userFriendlyMessage;
        AdditionalData = additionalData;
    }
}

