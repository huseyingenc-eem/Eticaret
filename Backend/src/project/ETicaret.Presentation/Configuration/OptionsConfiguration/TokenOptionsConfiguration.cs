using ETicaret.Application.Services.JwtServices;
using ETicaret.Presentation.Configuration.ServiceConfiguration;
using ETicaret.Presentation.Extensions;

namespace ETicaret.Presentation.Configuration.OptionsConfiguration;

/// <summary>
/// Token ayarlarını yapılandırır ve DI'a kaydeder.
/// </summary>
public class TokenOptionsConfiguration : IServiceConfiguration
{
    public int Order => 2;

    public IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // TokenOptions'ı güvenli şekilde oku
        var tokenOptions = configuration.GetRequiredSectionAs<CustomTokenOptions>("TokenOptions");

        // Singleton olarak kaydet
        services.AddSingleton(tokenOptions);

        // IOptions pattern için kaydet
        services.Configure<CustomTokenOptions>(configuration.GetSection("TokenOptions"));

        return services;
    }
}