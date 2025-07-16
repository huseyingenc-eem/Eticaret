using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Serilog;
using Serilog.Context;
using System.Security.Claims;

namespace Core.Shared.Helpers;

public static class LoggerHelper
{
    private static IHttpContextAccessor _httpContextAccessor;

    public static void Initialize(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public static class LoggerHelper
    {
        public static void LogWithContext(this ILogger logger, LogLevel level, string message,
            Exception exception = null, params object[] args)
        {
            using (LogContext.PushProperty("RequestId", GetRequestId()))
            using (LogContext.PushProperty("UserId", GetUserId()))
            using (LogContext.PushProperty("ActionName", GetActionName()))
            using (LogContext.PushProperty("ControllerName", GetControllerName()))
            {
                switch (level)
                {
                    case LogLevel.Information:
                        logger.Information(message, args);
                        break;
                    case LogLevel.Warning:
                        logger.Warning(message, args);
                        break;
                    case LogLevel.Error:
                        logger.Error(exception, message, args);
                        break;
                    case LogLevel.Debug:
                        logger.Debug(message, args);
                        break;
                    case LogLevel.Fatal:
                        logger.Fatal(exception, message, args);
                        break;
                    default:
                        logger.Information(message, args);
                        break;
                }
            }
        }

        public static void LogUserAction(this ILogger logger, string action, string details = null)
        {
            using (LogContext.PushProperty("RequestId", GetRequestId()))
            using (LogContext.PushProperty("UserId", GetUserId()))
            using (LogContext.PushProperty("ActionName", action))
            {
                logger.Information("User Action: {Action}. Details: {Details}", action, details ?? "No details");
            }
        }

        public static void LogPerformance(this ILogger logger, string operationName,
            TimeSpan duration, Dictionary<string, object> additionalData = null)
        {
            using (LogContext.PushProperty("RequestId", GetRequestId()))
            using (LogContext.PushProperty("UserId", GetUserId()))
            using (LogContext.PushProperty("ActionName", operationName))
            {
                var logMessage = "Performance: {OperationName} completed in {Duration}ms";
                var logArgs = new List<object> { operationName, duration.TotalMilliseconds };

                if (additionalData != null)
                {
                    foreach (var data in additionalData)
                    {
                        using (LogContext.PushProperty(data.Key, data.Value))
                        {
                            // Property context'e eklendi
                        }
                    }
                }

                logger.Information(logMessage, logArgs.ToArray());
            }
        }

        public static void LogDatabaseOperation(this ILogger logger, string operation,
            string tableName, int recordCount = 0, TimeSpan? duration = null)
        {
            using (LogContext.PushProperty("RequestId", GetRequestId()))
            using (LogContext.PushProperty("UserId", GetUserId()))
            using (LogContext.PushProperty("ActionName", $"DB_{operation}"))
            {
                var message = "Database Operation: {Operation} on {TableName}. Records: {RecordCount}";
                var args = new object[] { operation, tableName, recordCount };

                if (duration.HasValue)
                {
                    message += ". Duration: {Duration}ms";
                    args = args.Concat(new object[] { duration.Value.TotalMilliseconds }).ToArray();
                }

                logger.Information(message, args);
            }
        }

        public static void LogException(this ILogger logger, Exception exception,
            string context = null, Dictionary<string, object> additionalData = null)
        {
            using (LogContext.PushProperty("RequestId", GetRequestId()))
            using (LogContext.PushProperty("UserId", GetUserId()))
            using (LogContext.PushProperty("ActionName", context ?? "Exception"))
            {
                if (additionalData != null)
                {
                    foreach (var data in additionalData)
                    {
                        using (LogContext.PushProperty(data.Key, data.Value))
                        {
                            // Property context'e eklendi
                        }
                    }
                }

                logger.Error(exception, "Exception occurred in {Context}: {Message}",
                    context ?? "Unknown", exception.Message);
            }
        }

        // Yardımcı metodlar
        private static string GetRequestId()
        {
            // HttpContext'ten RequestId alın
            var httpContext = GetHttpContext();
            return httpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
        }

        private static string GetUserId()
        {
            var httpContext = GetHttpContext();
            return httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
        }

        private static string GetActionName()
        {
            var httpContext = GetHttpContext();
            return httpContext?.GetRouteValue("action")?.ToString() ?? "Unknown";
        }

        private static string GetControllerName()
        {
            var httpContext = GetHttpContext();
            return httpContext?.GetRouteValue("controller")?.ToString() ?? "Unknown";
        }

        private static HttpContext GetHttpContext()
        {
            return _httpContextAccessor?.HttpContext;
        }
    }

    public enum LogLevel
    {
        Debug,
        Information,
        Warning,
        Error,
        Fatal
    }
}