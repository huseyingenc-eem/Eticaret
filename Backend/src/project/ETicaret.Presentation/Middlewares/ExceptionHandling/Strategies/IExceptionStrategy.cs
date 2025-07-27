using Core.Application.Behaviors.Logging.Models;
using Core.Application.Abstractions.Services;

namespace ETicaret.Presentation.Middlewares.ExceptionHandling.Strategies;

/// <summary>
/// Exception handling stratejileri için temel arayüz.
/// Strategy pattern ile her exception türü için ayrı işleme mantığı sağlar.
/// </summary>
public interface IExceptionStrategy
{
    /// <summary>
    /// Bu strategy'nin belirtilen exception türünü işleyip işleyemeyeceğini kontrol eder.
    /// </summary>
    /// <param name="exception">Kontrol edilecek exception.</param>
    /// <returns>İşleyebilirse true, aksi halde false.</returns>
    bool CanHandle(Exception exception);

    /// <summary>
    /// Exception'ı işler ve uygun response döndürür.
    /// </summary>
    /// <param name="exception">İşlenecek exception.</param>
    /// <param name="context">HTTP context.</param>
    /// <param name="logDetail">Log detayları.</param>
    /// <param name="loggerService">Logger servisi.</param>
    /// <param name="isDevelopment">Development ortamında çalışıp çalışmadığı.</param>
    /// <returns>HTTP status code ve response object tuple'ı.</returns>
    Task<(int StatusCode, object Response)> HandleAsync(
        Exception exception,
        HttpContext context,
        LogDetail logDetail,
        ILoggerService loggerService,
        bool isDevelopment);
}