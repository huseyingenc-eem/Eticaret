using Core.Infrastructure.Logger.Serilog.ConfigurationModels;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace Core.Infrastructure.Logger.Serilog;

public class MsSqlLogger : LoggerServiceBase
{
    private readonly LoggingConfiguration _loggingConfiguration;

    public MsSqlLogger(LoggingConfiguration loggingConfiguration)
    {
        _loggingConfiguration = loggingConfiguration;
        MsSqlConfiguration logConfig = _loggingConfiguration.MsSqlConfiguration;

        MSSqlServerSinkOptions options = new MSSqlServerSinkOptions()
        {
            TableName = logConfig.TableName,
            AutoCreateSqlTable = logConfig.AutoCreateSqlTable
        };

        var columnOptions = new ColumnOptions();

        Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Is((LogEventLevel)Enum.Parse(typeof(LogEventLevel), _loggingConfiguration.MinimumLogLevel))
            .WriteTo.MSSqlServer(logConfig.ConnectionString, sinkOptions: options, columnOptions: columnOptions)
            .CreateLogger();
    }
}
