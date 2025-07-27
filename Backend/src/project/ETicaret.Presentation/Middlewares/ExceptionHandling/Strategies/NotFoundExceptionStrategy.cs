using Core.Application.Abstractions.Services;
using Core.Application.Behaviors.Logging.Models;
using Core.Application.Common.Exceptions;
using System.Text.Json;

namespace ETicaret.Presentation.Middlewares.ExceptionHandling.Strategies;

/// <summary>
/// NotFoundException'lar için strateji.
/// Kaynak bulunamadı hatalarını yönetir.
/// </summary>
public class NotFoundExceptionStrategy : IExceptionStrategy
{
    public bool CanHandle(Exception exception)
        => exception is NotFoundException;

    public async Task<(int StatusCode, object Response)> HandleAsync(
        Exception exception,
        HttpContext context,
        LogDetail logDetail,
        ILoggerService loggerService,
        bool isDevelopment)
    {
        var notFoundEx = (NotFoundException)exception;

        var response = new Dictionary<string, object>
        {
            ["type"] = "urn:ietf:rfc:7231#section-6.5.4",
            ["title"] = "Kaynak Bulunamadı",
            ["status"] = 404,
            ["detail"] = notFoundEx.Message,
            ["instance"] = context.Request.Path.ToString(),
            ["errorCode"] = notFoundEx.ErrorCode ?? "NOT_FOUND",
            ["userFriendlyMessage"] = notFoundEx.UserFriendlyMessage ?? "Aradığınız kaynak bulunamadı."
        };

        if (isDevelopment)
        {
            response["developerDetail"] = notFoundEx.ToString();
        }

        if (notFoundEx.AdditionalData != null)
        {
            response["additionalData"] = notFoundEx.AdditionalData;
        }

        logDetail.AdditionalExceptionData = notFoundEx.AdditionalData;
        loggerService.Warning(JsonSerializer.Serialize(logDetail, GetJsonOptions()));

        return (404, response);
    }

    private static JsonSerializerOptions GetJsonOptions() => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
}