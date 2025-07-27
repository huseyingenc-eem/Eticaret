using ETicaret.Presentation.Extensions;
using ETicaret.Presentation.Middlewares.ExceptionHandling;

namespace ETicaret.Presentation.Configuration.ServiceConfiguration;

/// <summary>
/// Exception handling servislerini yapılandırır.
/// </summary>
public class ExceptionHandlingServiceConfiguration : IServiceConfiguration
{
    public int Order => 3;

    public IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddExceptionHandlingStrategies();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}