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
            // Geliştirme ortamında hassas verilerin loglanmasını sağlar.
            opt.EnableSensitiveDataLogging();
        });

        // EKLEME: Core IUnitOfWork
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
        services.AddScoped<IContextualLogger, ContextualLogger>(); // ContextualLogger kaydını da ekleyelim.

        // 3. Scrutor kullanarak TÜM Repository'leri OTOMATİK OLARAK TARA VE KAYDET
        services.Scan(scan => scan
            // Bu projenin (ETicaret.Persistence) assembly'sini tara
            .FromAssemblyOf<PersistenceAssemblyReference>()
            // Adı "Repository" ile biten tüm somut sınıfları bul
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
            // Bulunan her sınıfı, kendi implemente ettiği arayüzüyle eşleştir
            // Örnek: IProductRepository -> ProductRepository
            .AsMatchingInterface()
            // ve tümünü Scoped olarak kaydet.
            .WithScopedLifetime()
        );

        return services;
    }
}