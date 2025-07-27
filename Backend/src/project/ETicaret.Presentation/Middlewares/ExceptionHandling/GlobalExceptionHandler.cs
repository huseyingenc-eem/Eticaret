using Core.Application.Abstractions.Services;
using Core.Application.Behaviors.Logging.Models;
using ETicaret.Presentation.Middlewares.ExceptionHandling.Extensions;
using ETicaret.Presentation.Middlewares.ExceptionHandling.Strategies;
using Microsoft.AspNetCore.Diagnostics;

namespace ETicaret.Presentation.Middlewares.ExceptionHandling;

/// <summary>
/// .NET 8'in IExceptionHandler arayüzünü kullanarak merkezi exception handling.
/// Strategy pattern ile modüler hale getirilmiş, Service Locator pattern kullanarak
/// Singleton-Scoped lifetime mismatch problemini çözer.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment _hostEnvironment;

    public GlobalExceptionHandler(IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // ✅ Service Locator Pattern: Runtime'da scoped servisleri al
        var loggerService = httpContext.RequestServices.GetRequiredService<ILoggerService>();
        var strategies = httpContext.RequestServices.GetServices<IExceptionStrategy>();

        var logDetail = httpContext.CreateLogDetail(exception);

        // Uygun strategy'yi bul ve çalıştır
        var strategy = strategies.FirstOrDefault(s => s.CanHandle(exception));

        if (strategy == null)
        {
            // Fallback strategy'yi bul (InternalServerExceptionStrategy her zaman true döner)
            strategy = strategies.LastOrDefault();
        }

        if (strategy == null)
        {
            // Son çare: Manuel internal server error response
            await HandleInternalServerErrorFallbackAsync(httpContext, exception, logDetail, loggerService, cancellationToken);
            return true;
        }

        var (statusCode, response) = await strategy.HandleAsync(
            exception,
            httpContext,
            logDetail,
            loggerService,
            _hostEnvironment.IsDevelopment()
        );

        // Response'u gönder
        await httpContext.WriteJsonResponseAsync(response, statusCode, cancellationToken);

        return true; // Exception işlendi
    }

    /// <summary>
    /// Strategy bulunamazsa fallback olarak kullanılan emergency response handler.
    /// </summary>
    private async Task HandleInternalServerErrorFallbackAsync(
        HttpContext httpContext,
        Exception exception,
        LogDetail logDetail,
        ILoggerService loggerService,
        CancellationToken cancellationToken)
    {
        var response = new Dictionary<string, object>
        {
            ["type"] = "urn:ietf:rfc:7231#section-6.6.1",
            ["title"] = "Sunucu Hatası",
            ["status"] = 500,
            ["detail"] = _hostEnvironment.IsDevelopment()
                ? exception.ToString()
                : "Beklenmedik bir sunucu hatası oluştu.",
            ["instance"] = httpContext.Request.Path.ToString(),
            ["errorCode"] = "INTERNAL_SERVER_ERROR"
        };

        if (_hostEnvironment.IsDevelopment())
        {
            response["stackTrace"] = exception.StackTrace;
            response["innerException"] = exception.InnerException?.ToString();
        }

        loggerService.Fatal($"EMERGENCY FALLBACK: {logDetail}", exception);

        await httpContext.WriteJsonResponseAsync(response, 500, cancellationToken);
    }
}