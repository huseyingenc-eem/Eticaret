using Core.Infrastructure.Services.Logging.Serilog.ConfigurationModels;

namespace Core.Infrastructure.Services.Logging;

public class LoggingConfiguration
{
    public FileLogConfiguration FileLogConfiguration { get; set; } = new FileLogConfiguration();
    public MsSqlConfiguration MsSqlConfiguration { get; set; } = new MsSqlConfiguration();
    public string MinimumLogLevel { get; set; } = "Information";
    // Diğer loglama ayarları buraya eklenebilir.
}
