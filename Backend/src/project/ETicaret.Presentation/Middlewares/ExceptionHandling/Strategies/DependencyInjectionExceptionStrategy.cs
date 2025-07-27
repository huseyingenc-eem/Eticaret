using Core.Application.Abstractions.Services;
using Core.Application.Behaviors.Logging.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ETicaret.Presentation.Middlewares.ExceptionHandling.Strategies;

/// <summary>
/// Dependency Injection hatalarını için strateji.
/// DI çözümleme hatalarını geliştirici dostu şekilde sunar.
/// </summary>
public class DependencyInjectionExceptionStrategy : IExceptionStrategy
{
    public bool CanHandle(Exception exception)
        => exception is InvalidOperationException invalidOpEx &&
           invalidOpEx.Message.Contains("Unable to resolve service");

    public async Task<(int StatusCode, object Response)> HandleAsync(
        Exception exception,
        HttpContext context,
        LogDetail logDetail,
        ILoggerService loggerService,
        bool isDevelopment)
    {
        var invalidOpEx = (InvalidOperationException)exception;

        var match = Regex.Match(invalidOpEx.Message,
            @"Unable to resolve service for type '(?<service>.*?)' while attempting to activate '(?<activator>.*?)'");

        string detailMessage = "Bir Dependency Injection (DI) hatası oluştu. Bir servis DI konteynerine kaydedilmemiş olabilir.";

        if (match.Success)
        {
            var serviceName = match.Groups["service"].Value.Split('.').Last();
            var activatorName = match.Groups["activator"].Value.Split('.').Last().Split('`').First();
            detailMessage = $"'{activatorName}' sınıfı, '{serviceName}' servisini talep ediyor ancak bu servis kaydedilmemiş. " +
                           "Lütfen ilgili servis kayıt dosyasını (örn: ApplicationServiceRegistration.cs) kontrol edin.";
        }

        var response = new Dictionary<string, object>
        {
            ["type"] = "urn:ietf:rfc:7231#section-6.6.1",
            ["title"] = "Dependency Injection Yapılandırma Hatası",
            ["status"] = 500,
            ["detail"] = detailMessage,
            ["instance"] = context.Request.Path.ToString(),
            ["errorCode"] = "DI_RESOLUTION_ERROR"
        };

        if (isDevelopment)
        {
            response["originalException"] = invalidOpEx.Message;
            response["stackTrace"] = invalidOpEx.StackTrace;
        }

        loggerService.Fatal(JsonSerializer.Serialize(logDetail, GetJsonOptions()), invalidOpEx);

        return (500, response);
    }

    private static JsonSerializerOptions GetJsonOptions() => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
}