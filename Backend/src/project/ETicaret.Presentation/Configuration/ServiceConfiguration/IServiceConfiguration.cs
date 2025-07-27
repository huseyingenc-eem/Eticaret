namespace ETicaret.Presentation.Configuration.ServiceConfiguration;

/// <summary>
/// Servis yapılandırma sınıfları için temel arayüz.
/// Tüm service configuration sınıfları bu arayüzü implemente etmelidir.
/// </summary>
public interface IServiceConfiguration
{
    /// <summary>
    /// Servisleri DI container'ına ekler.
    /// </summary>
    /// <param name="services">Servis koleksiyonu.</param>
    /// <param name="configuration">Yapılandırma ayarları.</param>
    /// <param name="environment">Çalışma ortamı bilgisi.</param>
    /// <returns>Yapılandırılmış servis koleksiyonu.</returns>
    IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment);

    /// <summary>
    /// Bu servis yapılandırmasının öncelik sırası.
    /// Düşük sayılar önce çalışır.
    /// </summary>
    int Order { get; }
}