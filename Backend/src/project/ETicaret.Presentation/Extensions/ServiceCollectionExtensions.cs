using ETicaret.Presentation.Configuration.ServiceConfiguration;
using ETicaret.Presentation.Configuration.MiddlewareConfiguration;
using ETicaret.Presentation.Configuration.StartupTasks;
using Core.Infrastructure.Extensions;
using ETicaret.Application;
using ETicaret.Persistence;
using Serilog;
using System.Reflection;

namespace ETicaret.Presentation.Extensions;

/// <summary>
/// Program.cs'i sadeleştirmek için yapılandırma extension'ları.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Tüm service configuration'ları otomatik olarak bulur ve sıralı şekilde uygular.
    /// </summary>
    public static IServiceCollection AddConfiguredServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // Önce layer service'lerini ekle (önemli sıralama!)
        services
            .AddApplicationServices(configuration)
            .AddInfrastructureServices(configuration)
            .AddPersistenceServices(configuration);

        // Service configuration'ları otomatik bul ve sırayla çalıştır
        var serviceConfigurations = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IServiceConfiguration).IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t) as IServiceConfiguration)
            .Where(c => c != null)
            .OrderBy(c => c!.Order)
            .ToList();

        foreach (var config in serviceConfigurations)
        {
            config!.ConfigureServices(services, configuration, environment);
        }

        return services;
    }

    /// <summary>
    /// Host builder'ı yapılandırır (Serilog vs.).
    /// </summary>
    public static WebApplicationBuilder ConfigureHost(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(Log.Logger);
        return builder;
    }

    /// <summary>
    /// Tüm middleware configuration'ları otomatik olarak bulur ve sıralı şekilde uygular.
    /// </summary>
    public static WebApplication UseConfiguredMiddlewares(this WebApplication app)
    {
        var middlewareConfigurations = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IMiddlewareConfiguration).IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t) as IMiddlewareConfiguration)
            .Where(c => c != null)
            .OrderBy(c => c!.Order)
            .ToList();

        foreach (var config in middlewareConfigurations)
        {
            config!.ConfigureMiddleware(app, app.Environment);
        }

        return app;
    }

    /// <summary>
    /// Startup task'ları çalıştırır.
    /// </summary>
    public static async Task RunStartupTasksAsync(this WebApplication app)
    {
        var startupTasks = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IStartupTask).IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t) as IStartupTask)
            .Where(t => t != null)
            .OrderBy(t => t!.Order)
            .ToList();

        foreach (var task in startupTasks)
        {
            await task!.ExecuteAsync(app);
        }
    }
}