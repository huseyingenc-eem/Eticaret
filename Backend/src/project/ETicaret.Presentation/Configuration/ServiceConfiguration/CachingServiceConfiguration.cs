namespace ETicaret.Presentation.Configuration.ServiceConfiguration;

/// <summary>
/// Redis cache servislerini yapılandırır.
/// </summary>
public class CachingServiceConfiguration : IServiceConfiguration
{
    public int Order => 5;

    public IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnectionString");
            options.InstanceName = "ETicaret_";
        });

        return services;
    }
}