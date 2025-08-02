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
using Microsoft.Extensions.Logging;
using Scrutor;
using Microsoft.Extensions.Options;

namespace ETicaret.Persistence;

/// <summary>
/// Persistence katmanı servislerini IServiceCollection'a eklemek için genişletme metotları içerir.
/// </summary>
public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BaseDBContexts>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("SqlConnection"));
            opt.EnableSensitiveDataLogging();
            opt.EnableDetailedErrors();

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

        services.Scan(scan => scan
            .FromAssemblyOf<PersistenceAssemblyReference>()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
            .AsMatchingInterface()
            .WithScopedLifetime()
        );

        return services;
    }
}