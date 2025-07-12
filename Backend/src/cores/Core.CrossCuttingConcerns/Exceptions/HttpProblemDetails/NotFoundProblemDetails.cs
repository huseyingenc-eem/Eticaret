using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Core.Shared.Exceptions.HttpProblemDetails;

public class NotFoundProblemDetails : ProblemDetails
{
    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; set; }

    [JsonPropertyName("userFriendlyMessage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? UserFriendlyMessage { get; set; }

    [JsonPropertyName("additionalData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? AdditionalData { get; set; }

    public NotFoundProblemDetails(string detail, string errorCode, string? userFriendlyMessage = null, object? additionalData = null)
    {
        Title = "Kaynak Bulunamadı";
        Detail = detail;
        Status = StatusCodes.Status404NotFound;
        Type = "urn:ietf:rfc:7231#section-6.5.4";
        ErrorCode = errorCode;
        UserFriendlyMessage = userFriendlyMessage;
        AdditionalData = additionalData;
    }
}