using Core.Shared.Exceptions;
using ETicaret.Presentation.Errors;
using Core.Shared.Logging.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Application.Abstractions.Services;

namespace ETicaret.Presentation.Middlewares
{
    public class HttpExceptionHandler
    {
        private readonly RequestDelegate _next;
        // ILoggerService kurucudan kaldırıldı
        private readonly IHostEnvironment _hostEnvironment;

        public HttpExceptionHandler(RequestDelegate next, IHostEnvironment hostEnvironment)
        {
            _next = next;
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
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

            if (exception is CoreException coreEx)
            {
                if (coreEx is FluentValidationException validationException)
                {
                    var validationErrors = validationException.Errors
                        .GroupBy(e => e.Property ?? "GeneralErrors")
                        .ToDictionary(
                            g => g.Key,
                            g => g.SelectMany(e => e.Errors ?? Enumerable.Empty<string>()).ToArray()
                        );

                    var problemDetails = new CustomValidationProblemDetails(
                        detail: validationException.UserFriendlyMessage ?? validationException.Message,
                        errorCode: validationException.ErrorCode,
                        userFriendlyMessage: validationException.UserFriendlyMessage,
                        validationErrors: validationErrors
                    )
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
                else if (coreEx is BusinessException businessException)
                {
                    var problemDetails = new BusinessProblemDetails(
                        detail: businessException.UserFriendlyMessage ?? businessException.Message,
                        errorCode: businessException.ErrorCode,
                        userFriendlyMessage: businessException.UserFriendlyMessage,
                        additionalData: businessException.AdditionalData
                    )
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "İş Kuralı Hatası",
                        Instance = context.Request.Path
                    };
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    logDetail.AdditionalExceptionData = businessException.AdditionalData;
                    loggerService.Info(JsonSerializer.Serialize(logDetail, GetJsonSerializerOptions()));
                    return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, GetJsonSerializerOptions()));
                }
                else if (coreEx is NotFoundException notFoundException)
                {
                    var problemDetails = new NotFoundProblemDetails(
                        detail: notFoundException.UserFriendlyMessage ?? notFoundException.Message,
                        errorCode: notFoundException.ErrorCode,
                        userFriendlyMessage: notFoundException.UserFriendlyMessage,
                        additionalData: notFoundException.AdditionalData
                    )
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "Kaynak Bulunamadı",
                        Instance = context.Request.Path
                    };
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    logDetail.AdditionalExceptionData = notFoundException.AdditionalData;
                    loggerService.Info(JsonSerializer.Serialize(logDetail, GetJsonSerializerOptions()));
                    return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, GetJsonSerializerOptions()));
                }
                else if (coreEx is AuthorizationException authorizationException)
                {
                    var problemDetails = new AuthorizationProblemDetails(
                        detail: authorizationException.UserFriendlyMessage ?? authorizationException.Message,
                        errorCode: authorizationException.ErrorCode,
                        userFriendlyMessage: authorizationException.UserFriendlyMessage,
                        additionalData: authorizationException.AdditionalData
                    )
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Title = "Yetkisiz Erişim",
                        Instance = context.Request.Path
                    };
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    logDetail.AdditionalExceptionData = authorizationException.AdditionalData;
                    loggerService.Info(JsonSerializer.Serialize(logDetail, GetJsonSerializerOptions()));
                    return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, GetJsonSerializerOptions()));
                }
                else
                {
                    var problemDetails = new ProblemDetails
                    {
                        Title = "Bir Hata Oluştu (Core Exception)",
                        Detail = _hostEnvironment.IsDevelopment() ? coreEx.ToString() : (coreEx.UserFriendlyMessage ?? coreEx.Message),
                        Status = StatusCodes.Status400BadRequest,
                        Instance = context.Request.Path,
                    };
                    problemDetails.Extensions["errorCode"] = coreEx.ErrorCode;
                    if (!string.IsNullOrEmpty(coreEx.UserFriendlyMessage))
                        problemDetails.Extensions["userFriendlyMessage"] = coreEx.UserFriendlyMessage;
                    if (coreEx.AdditionalData != null)
                        problemDetails.Extensions["additionalData"] = coreEx.AdditionalData;

                    logDetail.AdditionalExceptionData = coreEx.AdditionalData;
                    context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status400BadRequest;
                    loggerService.Error(JsonSerializer.Serialize(logDetail, GetJsonSerializerOptions()), coreEx);
                    return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, GetJsonSerializerOptions()));
                }
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var genericProblemDetails = new ProblemDetails
            {
                Title = "Sunucu Hatası",
                Detail = _hostEnvironment.IsDevelopment() ? exception.ToString() : "Beklenmedik bir sunucu hatası oluştu. Lütfen daha sonra tekrar deneyin.",
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path,
            };

            string unhandledErrorCode = Core.Shared.Constants.ErrorCodes.UnhandledException ?? "ERR_UNHANDLED";
            genericProblemDetails.Extensions["errorCode"] = unhandledErrorCode;

            loggerService.Fatal(JsonSerializer.Serialize(logDetail, GetJsonSerializerOptions()), exception);
            return context.Response.WriteAsync(JsonSerializer.Serialize(genericProblemDetails, GetJsonSerializerOptions()));
        }

        private LogDetail CreateLogDetail(HttpContext context, Exception exception)
        {
            var user = context.User;
            return new LogDetail
            {
                ClassFullName = exception.TargetSite?.DeclaringType?.FullName,
                MethodName = exception.TargetSite?.Name,
                User = user?.Identity?.IsAuthenticated == true ? user.Identity.Name : "Anonymous",
                UserId = user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
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
    }
}