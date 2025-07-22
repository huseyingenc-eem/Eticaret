using Core.Application;
using Core.Application.Abstractions.Services;
using ETicaret.Application.Services.RedisServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;


namespace ETicaret.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCoreApplicationServices();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddScoped<ICacheService, RedisCacheService>();

        services.Scan(scan => scan
            // DÜZELTME: Artık projemizi temsil eden temiz ve amacı belli olan
            // ApplicationAssemblyReference sınıfını referans olarak veriyoruz.
            .FromAssemblyOf<ApplicationAssemblyReference>()

            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                .AsMatchingInterface()
                .WithScopedLifetime()

            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Rules")))
                .AsSelf()
                .WithScopedLifetime()

            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Seeder")))
                .AsMatchingInterface()
                .WithScopedLifetime()
        );

        return services;
    }
}