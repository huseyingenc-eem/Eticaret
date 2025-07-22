using Core.Application.Abstractions.Services;
using Core.Infrastructure.Adapters;
using Core.Infrastructure.Services.DomainEvent;
using Core.Infrastructure.Services.Email;
using Core.Infrastructure.Services.Logging.Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Extensions;

/// <summary>
/// IServiceCollection için Infrastructure katmanına ait servisleri
/// merkezi olarak kaydeden genişletme metotları içerir.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Infrastructure katmanında tanımlanan tüm servisleri DI container'ına ekler.
    /// </summary>
    /// <param name="services">Servis koleksiyonu.</param>
    /// <param name="configuration">Uygulama yapılandırma ayarlarına erişim.</param>
    /// <returns>Yapılandırılmış servis koleksiyonu.</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Serileştirme Servisi
        // JSON serileştirme ve deserileştirme işlemlerini yapar.
        services.AddSingleton<ISerializerService, JsonSerializerService>();

        // 2. Olay Servisi
        // Domain olaylarını ilgili handler'lara yayınlar.
        services.AddScoped<IDomainEventService, DomainEventService>();

        // 3. E-posta Servisi
        // E-posta gönderme işlemlerini yönetir.
        services.AddScoped<IEmailService, EmailService>();

        // 4. Dosya Yönetim Servisi
        // Dosya yükleme, silme gibi işlemleri yönetir.
        //services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // 5. Loglama Servisleri
        AddLoggingServices(services);

        return services;
    }

    /// <summary>
    /// Loglama ile ilgili servisleri DI container'ına ekler.
    /// </summary>
    private static void AddLoggingServices(IServiceCollection services)
    {
        // Genel amaçlı loglama servisi.
        // ILogger arayüzü Serilog tarafından zaten enjekte edildiği için doğrudan kullanılabilir.
        services.AddSingleton<ILoggerService, LoggerServiceBase>();

        // Bağlamsal (UserId, RequestId vb.) loglama yapan servis.
        services.AddScoped<IContextualLogger, ContextualLogger>();
    }
}