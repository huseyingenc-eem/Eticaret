using Core.Application;
using Core.Application.Abstractions.Services;
using ETicaret.Application.Services.RedisServices;
using Core.Application.Behaviors.Rules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;
using FluentValidation;
using ETicaret.Application.Services.Authorization;

namespace ETicaret.Application;

/// <summary>
/// Uygulamanın ana iş mantığını içeren ETicaret.Application katmanına ait servisleri
/// ve bağımlılıkları Dependency Injection (DI) konteynerine kaydeder.
/// Bu sınıf, hem projeye özgü servisleri hem de Core.Application katmanından gelen
/// temel yapıları bir araya getirir.
/// </summary>
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        #region Core Altyapı Servisleri (Core Infrastructure Services)
        services.AddCoreApplicationServices();
        #endregion

        #region Application Katmanı Servisleri (Application Layer Services)
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        #endregion

        #region Önbellekleme Servisleri (Caching Services)
        services.AddScoped<ICacheService, RedisCacheService>();
        #endregion
        services.AddScoped<IAuthorizationRuleService, AuthorizationRuleService>();

        #region Otomatik Servis Kaydı (Scrutor - Automatic Service Registration)
        services.Scan(scan => scan
            // Taramayı bu projenin (ETicaret.Application) Assembly'si üzerinden yap.
            .FromAssemblyOf<ApplicationAssemblyReference>()

            // "...RuleSet" ile biten ve IRuleSet<> arayüzünü implemente eden sınıfları kaydet.
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Rules")))
                .AsSelf()
                .WithScopedLifetime()

            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                .AsMatchingInterface()
                .WithScopedLifetime()

            // "...Seeder" ile biten veri tohumlama sınıflarını kaydet.
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Seeder")))
                .AsMatchingInterface()
                .WithScopedLifetime()
        );
        #region Feature Rule Registration

        // Feature-specific rule registration'ları otomatik bul ve çalıştır
        RegisterFeatureRules(services);

        #endregion

        #endregion

        return services;
    }

    #region Helper Methods

    /// <summary>
    /// Tüm feature'ların rule registration'larını otomatik bulur ve kaydeder.
    /// </summary>
    private static void RegisterFeatureRules(IServiceCollection services)
    {
        var ruleRegistrations = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IRuleServiceRegistration).IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t) as IRuleServiceRegistration)
            .Where(r => r != null)
            .ToList();

        foreach (var registration in ruleRegistrations)
        {
            registration!.RegisterRules(services);
        }
    }

    #endregion
}