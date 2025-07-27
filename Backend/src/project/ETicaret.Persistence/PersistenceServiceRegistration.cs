using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Services;
using Core.Infrastructure.Services.Logging;
using Core.Infrastructure.Services.Logging.Serilog;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;
using ETicaret.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging; // ✅ Bu satırı ekleyin
using Scrutor;
using Microsoft.Extensions.Options;

namespace ETicaret.Persistence;

/// <summary>
/// Persistence katmanı servislerini IServiceCollection'a eklemek için genişletme metotları içerir.
/// </summary>
public static class PersistenceServiceRegistration
{
    /// <summary>
    /// Persistence katmanı için gerekli servisleri (DbContext, UnitOfWork, Repository'ler)
    /// Dependency Injection container'ına ekler.
    /// </summary>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Veritabanı context'ini (DbContext) kaydet.
        services.AddDbContext<BaseDBContexts>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("SqlConnection"));

            // ✅ SQL loglarını aktifleştir
            opt.EnableSensitiveDataLogging(); // Parametre değerlerini göster
            opt.EnableDetailedErrors(); // Detaylı hata mesajları

            // ✅ Console'a SQL logları yazdır (Development için)
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == "Development")
            {
                opt.LogTo(Console.WriteLine, new[] {
                    DbLoggerCategory.Database.Command.Name
                }, LogLevel.Information);

                Console.WriteLine("SQL Logging enabled for Development environment\n");
            }
        });
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddIdentity<User, IdentityRole>(opt =>
        {
            opt.User.RequireUniqueEmail = true;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredLength = 6;
        }).AddEntityFrameworkStores<BaseDBContexts>();

        services.Configure<LoggingConfiguration>(configuration.GetSection("SerilogLogConfigurations"));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<LoggingConfiguration>>().Value);
        services.AddScoped<ILoggerService, FileLogger>();
        services.AddScoped<IContextualLogger, ContextualLogger>();

        // 3. Scrutor kullanarak TÜM Repository'leri OTOMATİK OLARAK TARA VE KAYDET
        services.Scan(scan => scan
            .FromAssemblyOf<PersistenceAssemblyReference>()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
            .AsMatchingInterface()
            .WithScopedLifetime()
        );

        return services;
    }
}