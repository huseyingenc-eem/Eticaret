using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Services;
using Core.Infrastructure.Services.Logging;
using Core.Infrastructure.Services.Logging.Serilog;
using ETicaret.Domain.Entities;
using ETicaret.Persistence.Contexts;
using ETicaret.Persistence.Diagnostics;
using ETicaret.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scrutor;

namespace ETicaret.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SqlCommandLoggingOptions>(configuration.GetSection("SqlCommandLogging"));

        services.AddSingleton<SqlCommandLoggingOptions>(sp =>
            sp.GetRequiredService<IOptions<SqlCommandLoggingOptions>>().Value);

        // ---- DbContext Registration ----
        services.AddDbContext<BaseDBContexts>((sp, opt) =>
        {
            var connectionString = configuration.GetConnectionString("SqlConnection");
            opt.UseSqlServer(connectionString);

            var env = sp.GetRequiredService<IHostEnvironment>();
            var enableSqlLogging = configuration.GetValue<bool>("EnableSqlLogging", false);

            Console.WriteLine($"🌍 Environment: {env.EnvironmentName}");
            Console.WriteLine($"📊 EnableSqlLogging: {enableSqlLogging}");

            if (enableSqlLogging)
            {
                opt.EnableDetailedErrors();
                opt.EnableSensitiveDataLogging();

                var loggingOptions = sp.GetService<SqlCommandLoggingOptions>() ?? new SqlCommandLoggingOptions();
                var logger = sp.GetRequiredService<ILogger<SqlCommandLoggingInterceptor>>();
                var interceptor = new SqlCommandLoggingInterceptor(logger, loggingOptions);

                opt.AddInterceptors(interceptor);

                opt.LogTo(Console.WriteLine, LogLevel.Information);

                Console.WriteLine("✅ SQL command interceptor enabled\n");
            }
            else
            {
                Console.WriteLine("❌ SQL command interceptor disabled\n");
            }
        }, ServiceLifetime.Scoped);

        // ---- Other Services ----
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services
            .AddIdentity<User, IdentityRole>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<BaseDBContexts>();

        services.Configure<LoggingConfiguration>(configuration.GetSection("SerilogLogConfigurations"));
        services.AddSingleton<LoggingConfiguration>(sp =>
            sp.GetRequiredService<IOptions<LoggingConfiguration>>().Value);
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