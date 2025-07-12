using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Core.Shared.Exceptions.HttpProblemDetails;

/// <summary>
/// Doğrulama hataları (FluentValidationException) için HTTP yanıtında kullanılacak ProblemDetails yapısı.
/// RFC 7807 standardını temel alır ve doğrulama hatalarını içerir.
/// </summary>
public class CustomValidationProblemDetails : ProblemDetails
{
    /// <summary>
    /// Uygulama genelinde benzersiz hata kodu (genellikle VALIDATION_ERROR).
    /// </summary>
    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; set; }

    /// <summary>
    /// Hatanın kullanıcıya gösterilebilecek, yerelleştirilmiş mesajı (isteğe bağlı).
    /// </summary>
    [JsonPropertyName("userFriendlyMessage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? UserFriendlyMessage { get; set; }

    /// <summary>
    /// Doğrulama hatalarını içeren bir sözlük.
    /// Anahtar, hatalı özelliğin adıdır. Değer, o özelliğe ait hata mesajları listesidir.
    /// </summary>
    [JsonPropertyName("errors")]
    public IDictionary<string, string[]> ValidationErrors { get; set; }

    public CustomValidationProblemDetails(
        string detail,
        string errorCode,
        IDictionary<string, string[]> validationErrors,
        string? userFriendlyMessage = null)
    {
        Title = "Doğrulama Hatası";
        Detail = detail; // Ana exception mesajı
        Status = StatusCodes.Status400BadRequest;
        Type = "urn:ietf:rfc:7231#section-6.5.1"; // Genel Bad Request veya özel bir Type URI
        ErrorCode = errorCode;
        UserFriendlyMessage = userFriendlyMessage;
        ValidationErrors = validationErrors;
    }
}
