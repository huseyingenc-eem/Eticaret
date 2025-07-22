using Core.Application.Abstractions.Services;
using Core.Application.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using System.Security.Claims;

namespace Core.Infrastructure.Services.Logging.Serilog;

public class ContextualLogger : IContextualLogger
{
    private readonly ILogger _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContextualLogger(ILogger logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    // Artık "Serilog.Events" ön ekine gerek yok.
    public void Verbose(string messageTemplate, params object[]? propertyValues) => Write(LogEventLevel.Verbose, null, messageTemplate, propertyValues);
    public void Debug(string messageTemplate, params object[]? propertyValues) => Write(LogEventLevel.Debug, null, messageTemplate, propertyValues);
    public void Information(string messageTemplate, params object[]? propertyValues) => Write(LogEventLevel.Information, null, messageTemplate, propertyValues);
    public void Warning(string messageTemplate, params object[]? propertyValues) => Write(LogEventLevel.Warning, null, messageTemplate, propertyValues);
    public void Error(Exception? exception, string messageTemplate, params object[]? propertyValues) => Write(LogEventLevel.Error, exception, messageTemplate, propertyValues);
    public void Fatal(Exception? exception, string messageTemplate, params object[]? propertyValues) => Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValues);

    private void Write(LogEventLevel level, Exception? exception, string messageTemplate, params object[]? propertyValues)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            _logger.Write(level, exception, messageTemplate, propertyValues);
            return;
        }

        using (LogContext.PushProperty("CorrelationId", httpContext.TraceIdentifier))
        using (LogContext.PushProperty("UserId", httpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous"))
        using (LogContext.PushProperty("ClientIp", httpContext.Connection.RemoteIpAddress?.ToString()))
        {
            _logger.Write(level, exception, messageTemplate, propertyValues);
        }
    }
}