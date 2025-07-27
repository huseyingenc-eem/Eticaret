namespace ETicaret.Presentation.Configuration.ServiceConfiguration;

/// <summary>
/// CORS politikalarını yapılandırır.
/// </summary>
public class CorsServiceConfiguration : IServiceConfiguration
{
    public int Order => 6;

    public const string ReactCorsPolicy = "ReactCorsPolicy";

    public IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(ReactCorsPolicy, policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }
}