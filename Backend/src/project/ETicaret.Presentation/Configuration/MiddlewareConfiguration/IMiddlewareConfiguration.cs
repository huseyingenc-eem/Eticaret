namespace ETicaret.Presentation.Configuration.MiddlewareConfiguration;

/// <summary>
/// Middleware yapılandırma sınıfları için temel arayüz.
/// </summary>
public interface IMiddlewareConfiguration
{
    /// <summary>
    /// Middleware'leri pipeline'a ekler.
    /// </summary>
    /// <param name="app">Web application builder.</param>
    /// <param name="environment">Çalışma ortamı bilgisi.</param>
    /// <returns>Yapılandırılmış web application.</returns>
    WebApplication ConfigureMiddleware(WebApplication app, IWebHostEnvironment environment);

    /// <summary>
    /// Bu middleware yapılandırmasının öncelik sırası.
    /// </summary>
    int Order { get; }
}