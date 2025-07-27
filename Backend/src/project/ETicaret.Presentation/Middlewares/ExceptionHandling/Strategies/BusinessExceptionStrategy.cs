using Core.Application.Abstractions.Services;
using Core.Application.Behaviors.Logging.Models;
using Core.Application.Common.Exceptions;
using System.Text.Json;

namespace ETicaret.Presentation.Middlewares.ExceptionHandling.Strategies;

/// <summary>
/// Business exception'ları için strateji.
/// İş kuralı ihlallerini kullanıcı dostu şekilde sunar.
/// </summary>
public class BusinessExceptionStrategy : IExceptionStrategy
{
    public bool CanHandle(Exception exception)
        => exception is BusinessException;

    public async Task<(int StatusCode, object Response)> HandleAsync(
        Exception exception,
        HttpContext context,
        LogDetail logDetail,
        ILoggerService loggerService,
        bool isDevelopment)
    {
        var businessEx = (BusinessException)exception;

        var response = new Dictionary<string, object>
        {
            ["type"] = "urn:ietf:rfc:7231#section-6.5.1",
            ["title"] = "İş Kuralı İhlali",
            ["status"] = 400,
            ["detail"] = businessEx.Message,
            ["instance"] = context.Request.Path.ToString(),
            ["errorCode"] = businessEx.ErrorCode ?? "BUSINESS_RULE_VIOLATION",
            ["userFriendlyMessage"] = businessEx.UserFriendlyMessage ?? businessEx.Message
        };

        if (isDevelopment)
        {
            response["developerDetail"] = businessEx.ToString();
        }

        if (businessEx.AdditionalData != null)
        {
            response["additionalData"] = businessEx.AdditionalData;
        }

        logDetail.AdditionalExceptionData = businessEx.AdditionalData;
        loggerService.Error(JsonSerializer.Serialize(logDetail, GetJsonOptions()), businessEx);

        return (400, response);
    }

    private static JsonSerializerOptions GetJsonOptions() => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
}