using Core.Application.Abstractions.Services;
using Core.Application.Behaviors.Logging.Models;
using Core.Application.Common.Exceptions;
using System.Text.Json;

namespace ETicaret.Presentation.Middlewares.ExceptionHandling.Strategies;

/// <summary>
/// Authorization exception'ları için strateji.
/// Yetkilendirme hatalarını yönetir.
/// </summary>
public class AuthorizationExceptionStrategy : IExceptionStrategy
{
    public bool CanHandle(Exception exception)
        => exception is AuthorizationException;

    public async Task<(int StatusCode, object Response)> HandleAsync(
        Exception exception,
        HttpContext context,
        LogDetail logDetail,
        ILoggerService loggerService,
        bool isDevelopment)
    {
        var authEx = (AuthorizationException)exception;

        var response = new Dictionary<string, object>
        {
            ["type"] = "urn:ietf:rfc:7231#section-6.5.3",
            ["title"] = "Yetkisiz Erişim",
            ["status"] = 403,
            ["detail"] = authEx.Message,
            ["instance"] = context.Request.Path.ToString(),
            ["errorCode"] = authEx.ErrorCode ?? "FORBIDDEN",
            ["userFriendlyMessage"] = authEx.UserFriendlyMessage ?? "Bu işlem için yetkiniz bulunmuyor."
        };

        if (isDevelopment)
        {
            response["developerDetail"] = authEx.ToString();
        }

        if (authEx.AdditionalData != null)
        {
            response["additionalData"] = authEx.AdditionalData;
        }

        logDetail.AdditionalExceptionData = authEx.AdditionalData;
        loggerService.Warning(JsonSerializer.Serialize(logDetail, GetJsonOptions()));

        return (403, response);
    }

    private static JsonSerializerOptions GetJsonOptions() => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
}