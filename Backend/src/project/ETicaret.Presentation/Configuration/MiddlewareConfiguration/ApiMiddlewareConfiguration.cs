namespace ETicaret.Presentation.Configuration.MiddlewareConfiguration;

/// <summary>
/// API middleware'lerini yapılandırır.
/// </summary>
public class ApiMiddlewareConfiguration : IMiddlewareConfiguration
{
    public int Order => 3;

    public WebApplication ConfigureMiddleware(WebApplication app, IWebHostEnvironment environment)
    {
        app.MapControllers();
        return app;
    }
}