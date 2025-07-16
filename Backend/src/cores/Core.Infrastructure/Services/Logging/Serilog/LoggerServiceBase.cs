using Serilog;
using Core.Application.Abstractions.Services;

namespace Core.Infrastructure.Services.Logging.Serilog;

public class LoggerServiceBase : ILoggerService
{

    public LoggerServiceBase()
    {
        Logger = default;
    }

    protected ILogger Logger { get; set; }

    public LoggerServiceBase(ILogger logger)
    {
        Logger = logger;
    }

    public void Debug(string message) => Logger.Debug(message);

    public void Error(string message, Exception exception = null) => Logger.Error(exception, message);

    public void Fatal(string message, Exception exception = null) => Logger.Fatal(exception, message);

    public void Info(string message)  => Logger.Information(message);

    public void Warning(string message)  => Logger.Warning(message);

}
