using ETicaret.Presentation.Configuration.ServiceConfiguration;

namespace ETicaret.Presentation.Configuration.MiddlewareConfiguration;

/// <summary>
/// Güvenlik middleware'lerini yapılandırır.
/// </summary>
public class SecurityMiddlewareConfiguration : IMiddlewareConfiguration
{
    public int Order => 2;

    public WebApplication ConfigureMiddleware(WebApplication app, IWebHostEnvironment environment)
    {
        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseCors(CorsServiceConfiguration.ReactCorsPolicy);
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}