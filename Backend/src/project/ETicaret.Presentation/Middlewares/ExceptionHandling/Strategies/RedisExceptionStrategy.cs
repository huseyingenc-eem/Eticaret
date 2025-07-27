using Core.Application.Abstractions.Services;
using Core.Application.Behaviors.Logging.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace ETicaret.Presentation.Middlewares.ExceptionHandling.Strategies;

/// <summary>
/// Redis exception'ları için strateji.
/// Redis bağlantı ve cache işlem hatalarını yönetir.
/// </summary>
public class RedisExceptionStrategy : IExceptionStrategy
{
    public bool CanHandle(Exception exception)
    {
        // Ana exception'ı kontrol et
        if (exception is RedisException ||
            exception is RedisTimeoutException ||
            exception is RedisConnectionException)
            return true;

        // Inner exception'ı da kontrol et
        var innerException = exception.InnerException;
        while (innerException != null)
        {
            if (innerException is RedisException ||
                innerException is RedisTimeoutException ||
                innerException is RedisConnectionException)
                return true;

            innerException = innerException.InnerException;
        }

        // Exception mesajında Redis hatası var mı kontrol et
        return exception.Message.Contains("redis", StringComparison.OrdinalIgnoreCase) ||
               exception.Message.Contains("RedisConnectionException", StringComparison.OrdinalIgnoreCase) ||
               exception.StackTrace?.Contains("Microsoft.Extensions.Caching.StackExchangeRedis") == true;
    }

    public async Task<(int StatusCode, object Response)> HandleAsync(
        Exception exception,
        HttpContext context,
        LogDetail logDetail,
        ILoggerService loggerService,
        bool isDevelopment)
    {
        var response = new Dictionary<string, object>
        {
            ["type"] = "https://tools.ietf.org/html/rfc7231#section-6.6.4",
            ["title"] = "Redis Servisi Hatası",
            ["status"] = 503,
            ["detail"] = "Önbellek servisi bağlantısı kurulamadı.",
            ["instance"] = context.Request.Path.ToString(),
            ["errorCode"] = "REDIS_CONNECTION_ERROR",
            ["userFriendlyMessage"] = "Sistem geçici olarak yavaş çalışabilir. Lütfen daha sonra tekrar deneyin."
        };

        if (isDevelopment)
        {
            response["developerDetail"] = exception.ToString();
            response["redisDetails"] = new
            {
                Message = exception.Message,
                InnerExceptionType = exception.InnerException?.GetType().Name,
                InnerExceptionMessage = exception.InnerException?.Message
            };
        }

        logDetail.AdditionalExceptionData = new
        {
            RedisError = true,
            OriginalExceptionType = exception.GetType().Name,
            RedisRelatedStackTrace = exception.StackTrace?.Contains("StackExchangeRedis")
        };

        loggerService.Warning(JsonSerializer.Serialize(logDetail, GetJsonOptions()));

        return (503, response);
    }

    private static JsonSerializerOptions GetJsonOptions() => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
}