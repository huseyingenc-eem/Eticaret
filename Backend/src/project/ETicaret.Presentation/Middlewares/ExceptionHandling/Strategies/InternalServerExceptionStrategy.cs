using Core.Application.Abstractions.Services;
using Core.Application.Behaviors.Logging.Models;
using Core.Application.Common.Constants;
using System.Text.Json;

namespace ETicaret.Presentation.Middlewares.ExceptionHandling.Strategies;

/// <summary>
/// Genel exception'lar için varsayılan strateji.
/// Beklenmeyen hataları güvenli şekilde yönetir.
/// </summary>
public class InternalServerExceptionStrategy : IExceptionStrategy
{
    public bool CanHandle(Exception exception)
        => true; // Bu her zaman son çare olarak çalışır

    public async Task<(int StatusCode, object Response)> HandleAsync(
        Exception exception,
        HttpContext context,
        LogDetail logDetail,
        ILoggerService loggerService,
        bool isDevelopment)
    {
        var response = new Dictionary<string, object>
        {
            ["type"] = "urn:ietf:rfc:7231#section-6.6.1",
            ["title"] = "Sunucu Hatası",
            ["status"] = 500,
            ["detail"] = isDevelopment ? exception.ToString() : "Beklenmedik bir sunucu hatası oluştu.",
            ["instance"] = context.Request.Path.ToString(),
            ["errorCode"] = ApplicationErrorCodes.UnhandledException
        };

        if (isDevelopment)
        {
            response["stackTrace"] = exception.StackTrace;
            response["innerException"] = exception.InnerException?.ToString();
        }

        loggerService.Fatal(JsonSerializer.Serialize(logDetail, GetJsonOptions()), exception);

        return (500, response);
    }

    private static JsonSerializerOptions GetJsonOptions() => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
}