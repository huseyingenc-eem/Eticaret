using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails;


/// <summary>
/// İş mantığı hataları (BusinessException) için HTTP yanıtında kullanılacak ProblemDetails yapısı.
/// RFC 7807 standardını temel alır ve özel alanlar içerir.
/// </summary>
public class BusinessProblemDetails : ProblemDetails
{

    /// <summary>
    /// Uygulama genelinde benzersiz hata kodu.
    /// </summary>
    [JsonPropertyName("errorCode")] // JSON yanıtında "errorCode" olarak görünmesi için
    public string ErrorCode { get; set; }

    /// <summary>
    /// Hatanın kullanıcıya gösterilebilecek, yerelleştirilmiş mesajı (isteğe bağlı).
    /// </summary>
    [JsonPropertyName("userFriendlyMessage")]
    public string? UserFriendlyMessage { get; set; }



    /// <summary>
    /// Hata ile ilgili ek, yapılandırılmış veriler (isteğe bağlı).
    /// </summary>
    [JsonPropertyName("additionalData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // Null ise JSON'a dahil etme
    public object? AdditionalData { get; set; }


    public BusinessProblemDetails(string detail, string errorCode, string? userFriendlyMessage = null, object? additionalData = null)
    {
        Title = "İş Kuralı İhlali";
        Detail = detail; 
        Status = StatusCodes.Status400BadRequest;
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        ErrorCode = errorCode;
        UserFriendlyMessage = userFriendlyMessage;
        AdditionalData = additionalData;
    }
}
