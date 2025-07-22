using Core.Application.Abstractions.Services;
using Core.Application.Behaviors.Logging.Models;
using Core.Application.Common.Constants;
using Core.Application.Common.Exceptions;
using ETicaret.Presentation.Errors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ETicaret.Presentation.Middlewares;

public class HttpExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _hostEnvironment;

    public HttpExceptionHandler(RequestDelegate next, IHostEnvironment hostEnvironment)
    {
        _next = next;
        _hostEnvironment = hostEnvironment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var loggerService = context.RequestServices.GetRequiredService<ILoggerService>();
            await HandleExceptionAsync(context, ex, loggerService);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception, ILoggerService loggerService)
    {
        context.Response.ContentType = "application/problem+json";
        LogDetail logDetail = CreateLogDetail(context, exception);

        // DÜZELTME: 'ApplicationException' yazdığımızda, System'in değil, BİZİM
        // sınıfımızı kullanmasını sağlamak için tam namespace'ini belirtiyoruz.
        if (exception is Core.Application.Common.Exceptions.ApplicationException applicationException)
        {
            return applicationException switch
            {
                FluentValidationException ex => CreateValidationProblemDetailsResponse(context, ex, logDetail, loggerService),
                BusinessException ex => CreateProblemDetailsResponse(context, ex, "İş Kuralı Hatası", StatusCodes.Status400BadRequest, logDetail, loggerService),
                NotFoundException ex => CreateProblemDetailsResponse(context, ex, "Kaynak Bulunamadı", StatusCodes.Status404NotFound, logDetail, loggerService),
                AuthorizationException ex => CreateProblemDetailsResponse(context, ex, "Yetkisiz Erişim", StatusCodes.Status403Forbidden, logDetail, loggerService),
                _ => CreateProblemDetailsResponse(context, applicationException, "Uygulama Hatası", StatusCodes.Status400BadRequest, logDetail, loggerService)
            };
        }

        return CreateInternalServerErrorResponse(context, exception, logDetail, loggerService);
    }

    #region Yardımcı Metotlar

    private Task CreateValidationProblemDetailsResponse(HttpContext context, FluentValidationException exception, LogDetail logDetail, ILoggerService loggerService)
    {
        var validationErrors = exception.Errors
            .GroupBy(e => e.Property ?? "GeneralErrors")
            .ToDictionary(g => g.Key, g => g.SelectMany(e => e.Errors ?? Enumerable.Empty<string>()).ToArray());

        var problemDetails = new CustomValidationProblemDetails(
            detail: exception.UserFriendlyMessage,
            errorCode: exception.ErrorCode,
            userFriendlyMessage: exception.UserFriendlyMessage,
            validationErrors: validationErrors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Doğrulama Hatası Oluştu",
            Instance = context.Request.Path
        };
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        logDetail.AdditionalExceptionData = problemDetails.ValidationErrors;
        loggerService.Info(JsonSerializer.Serialize(logDetail, GetJsonSerializerOptions()));

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, GetJsonSerializerOptions()));
    }

    // DÜZELTME: Metot parametresinde de belirsizliği gidermek için tam namespace kullanıyoruz.
    private Task CreateProblemDetailsResponse(HttpContext context, Core.Application.Common.Exceptions.ApplicationException exception, string title, int statusCode, LogDetail logDetail, ILoggerService loggerService)
    {
        context.Response.StatusCode = statusCode;
        var problemDetails = new ProblemDetails
        {
            Title = title,
            Detail = _hostEnvironment.IsDevelopment() ? exception.ToString() : exception.UserFriendlyMessage,
            Status = statusCode,
            Instance = context.Request.Path,
        };
        problemDetails.Extensions["errorCode"] = exception.ErrorCode;
        problemDetails.Extensions["userFriendlyMessage"] = exception.UserFriendlyMessage;

        logDetail.AdditionalExceptionData = exception.AdditionalData;
        loggerService.Error(JsonSerializer.Serialize(logDetail, GetJsonSerializerOptions()), exception);

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, GetJsonSerializerOptions()));
    }

    // ... (Geri kalan tüm yardımcı metotlarınız aynı kalabilir) ...
    private Task CreateInternalServerErrorResponse(HttpContext context, Exception exception, LogDetail logDetail, ILoggerService loggerService)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var problemDetails = new ProblemDetails
        {
            Title = "Sunucu Hatası",
            Detail = _hostEnvironment.IsDevelopment() ? exception.ToString() : "Beklenmedik bir sunucu hatası oluştu.",
            Status = StatusCodes.Status500InternalServerError,
            Instance = context.Request.Path,
        };
        problemDetails.Extensions["errorCode"] = ApplicationErrorCodes.UnhandledException;

        loggerService.Fatal(JsonSerializer.Serialize(logDetail, GetJsonSerializerOptions()), exception);

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, GetJsonSerializerOptions()));
    }

    private LogDetail CreateLogDetail(HttpContext context, Exception exception)
    {
        var user = context.User;
        return new LogDetail
        {
            ClassFullName = exception.TargetSite?.DeclaringType?.FullName,
            MethodName = exception.TargetSite?.Name,
            User = user?.Identity?.IsAuthenticated == true ? user.Identity.Name : "Anonymous",
            UserId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            RequestPath = context.Request.Path.ToString(),
            RequestMethod = context.Request.Method,
            ClientIpAddress = context.Connection.RemoteIpAddress?.ToString(),
            LogTime = DateTime.UtcNow,
            ExceptionType = exception.GetType().FullName,
            ExceptionMessage = exception.Message,
            ExceptionStackTrace = _hostEnvironment.IsDevelopment() ? exception.StackTrace : null,
            ExceptionDetails = _hostEnvironment.IsDevelopment() ? exception.ToString() : exception.Message,
        };
    }

    private JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = _hostEnvironment.IsDevelopment()
        };
    }
    #endregion
}