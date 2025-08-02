using AutoMapper;
using Core.Application;
using Core.Application.Abstractions.Services;
using Core.Application.Behaviors.Rules;
using ETicaret.Application.Common.Mappings;
using ETicaret.Application.Services.Authorization;
using ETicaret.Application.Services.RedisServices;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

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

        // ✅ AutoMapper'ı feature-based mapper'larla birlikte kaydet
        services.AddAutoMapper(cfg =>
        {
            // 1. Ana MappingProfile'ı ekle (IMapFrom interface'leri için)
            cfg.AddProfile<MappingProfile>();

            // 2. Feature-specific Profile'ları otomatik bul ve ekle
            var assembly = Assembly.GetExecutingAssembly();
            var profileTypes = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Profile)) &&
                           !t.IsAbstract &&
                           t != typeof(MappingProfile)) // Ana profile'ı hariç tut
                .ToList();

            foreach (var profileType in profileTypes)
            {
                cfg.AddProfile(profileType);
                Console.WriteLine($"✅ AutoMapper Profile registered: {profileType.Name}");
            }
        }, Assembly.GetExecutingAssembly());

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

            .AddClasses(classes => classes.AssignableTo(typeof(IBusinessRule<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        #endregion

        return services;
    }

    #region Helper Methods

    #endregion
}