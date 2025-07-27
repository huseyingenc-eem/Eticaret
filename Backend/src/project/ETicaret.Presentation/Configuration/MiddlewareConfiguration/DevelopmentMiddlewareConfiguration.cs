namespace ETicaret.Presentation.Configuration.MiddlewareConfiguration;

/// <summary>
/// Development ortamına özel middleware'leri yapılandırır.
/// </summary>
public class DevelopmentMiddlewareConfiguration : IMiddlewareConfiguration
{
    public int Order => 1;

    public WebApplication ConfigureMiddleware(WebApplication app, IWebHostEnvironment environment)
    {

        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ETicaret API v1");
                c.RoutePrefix = "swagger";
            });
        }
        else
        {
            Console.WriteLine("⚠️  Not in Development environment, Swagger not added.");
        }

        return app;
    }
}