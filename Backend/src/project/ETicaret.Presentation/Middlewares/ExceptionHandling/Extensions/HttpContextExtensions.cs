using Core.Application.Behaviors.Logging.Models;
using System.Security.Claims;
using System.Text.Json;

namespace ETicaret.Presentation.Middlewares.ExceptionHandling.Extensions;

/// <summary>
/// HttpContext için extension metodlar.
/// Exception handling sürecinde kullanılan yardımcı metodlar.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// HttpContext'ten LogDetail oluşturur.
    /// </summary>
    public static LogDetail CreateLogDetail(this HttpContext context, Exception exception)
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
            ExceptionStackTrace = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                ? exception.StackTrace : null,
            ExceptionDetails = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                ? exception.ToString() : exception.Message,
        };
    }

    /// <summary>
    /// JSON response'u HTTP context'e yazar.
    /// </summary>
    public static async Task WriteJsonResponseAsync(
        this HttpContext context,
        object responseModel,
        int statusCode,
        CancellationToken cancellationToken = default)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
        };

        var jsonResponse = JsonSerializer.Serialize(responseModel, jsonOptions);
        await context.Response.WriteAsync(jsonResponse, cancellationToken);
    }
}