using Core.Infrastructure.Logger.Serilog.ConfigurationModels;

namespace Core.Infrastructure.Logger;

public class LoggingConfiguration
{
    public FileLogConfiguration FileLogConfiguration { get; set; } = new FileLogConfiguration();
    public MsSqlConfiguration MsSqlConfiguration { get; set; } = new MsSqlConfiguration();
    public string MinimumLogLevel { get; set; } = "Information";
    // Diğer loglama ayarları buraya eklenebilir.
}
