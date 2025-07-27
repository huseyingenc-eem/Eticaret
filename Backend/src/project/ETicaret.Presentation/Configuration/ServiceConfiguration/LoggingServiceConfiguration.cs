using Serilog;

namespace ETicaret.Presentation.Configuration.ServiceConfiguration;

/// <summary>
/// Serilog loglama servislerini yapılandırır.
/// En yüksek öncelikli servis (Order = 1).
/// </summary>
public class LoggingServiceConfiguration : IServiceConfiguration
{
    public int Order => 1;

    public IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // Serilog yapılandırması
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            .CreateLogger();

        return services;
    }
}