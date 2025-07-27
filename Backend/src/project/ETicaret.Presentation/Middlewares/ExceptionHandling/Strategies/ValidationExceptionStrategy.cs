using Core.Application.Abstractions.Services;
using Core.Application.Behaviors.Logging.Models;
using Core.Application.Common.Exceptions;
using System.Text.Json;

namespace ETicaret.Presentation.Middlewares.ExceptionHandling.Strategies;

/// <summary>
/// FluentValidation exception'ları için strateji.
/// Validation hatalarını kullanıcı dostu formatta düzenler.
/// </summary>
public class ValidationExceptionStrategy : IExceptionStrategy
{
    public bool CanHandle(Exception exception)
        => exception is FluentValidationException;

    public async Task<(int StatusCode, object Response)> HandleAsync(
        Exception exception,
        HttpContext context,
        LogDetail logDetail,
        ILoggerService loggerService,
        bool isDevelopment)
    {
        var validationEx = (FluentValidationException)exception;

        // Validation hatalarını Dictionary<string, string[]> formatına çevir
        var validationErrors = validationEx.Errors
            .GroupBy(e => e.Property)
            .ToDictionary(
                g => g.Key ?? "GeneralErrors",
                g => g.SelectMany(e => e.Errors).Distinct().ToArray()
            );

        var response = new
        {
            type = "urn:ietf:rfc:7231#section-6.5.1",
            title = "Doğrulama Hatası Oluştu",
            status = 400,
            detail = validationEx.UserFriendlyMessage ?? "Lütfen girdiğiniz bilgileri kontrol ediniz.",
            instance = context.Request.Path.ToString(),
            errors = validationErrors,
            errorCode = validationEx.ErrorCode ?? "VALIDATION_ERROR"
        };

        // Loglama
        logDetail.AdditionalExceptionData = validationErrors;
        loggerService.Info(JsonSerializer.Serialize(logDetail, GetJsonOptions()));

        return (400, response);
    }

    private static JsonSerializerOptions GetJsonOptions() => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
}