using Core.Application.Abstractions.Services;
using Core.Application.Behaviors.Logging.Models;
using Core.Application.Common.Exceptions;
using System.Text.Json;

namespace ETicaret.Presentation.Middlewares.ExceptionHandling.Strategies;

/// <summary>
/// Authentication exception'ları için strateji.
/// 401 Unauthorized hatalarını yönetir.
/// </summary>
public class AuthenticationExceptionStrategy : IExceptionStrategy
{
    public bool CanHandle(Exception exception)
        => exception is AuthenticationException;

    public async Task<(int StatusCode, object Response)> HandleAsync(
        Exception exception,
        HttpContext context,
        LogDetail logDetail,
        ILoggerService loggerService,
        bool isDevelopment)
    {
        var authEx = (AuthenticationException)exception;

        var response = new Dictionary<string, object>
        {
            ["type"] = "urn:ietf:rfc:7235#section-3.1",
            ["title"] = "Kimlik Doğrulama Gerekli",
            ["status"] = 401,
            ["detail"] = authEx.Message,
            ["instance"] = context.Request.Path.ToString(),
            ["errorCode"] = authEx.ErrorCode ?? "AUTHENTICATION_REQUIRED",
            ["userFriendlyMessage"] = authEx.UserFriendlyMessage ?? "Oturum açmanız gerekiyor."
        };

        if (isDevelopment)
        {
            response["developerDetail"] = authEx.ToString();
        }

        if (authEx.AdditionalData != null)
        {
            response["additionalData"] = authEx.AdditionalData;
        }

        // Set WWW-Authenticate header
        context.Response.Headers.Append("WWW-Authenticate", "Bearer");

        logDetail.AdditionalExceptionData = authEx.AdditionalData;
        loggerService.Warning(JsonSerializer.Serialize(logDetail, GetJsonOptions()));

        return (401, response);
    }

    private static JsonSerializerOptions GetJsonOptions() => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
}