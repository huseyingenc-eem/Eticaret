using Microsoft.OpenApi.Models;

namespace ETicaret.Presentation.Configuration.ServiceConfiguration;

/// <summary>
/// Swagger/OpenAPI servislerini yapılandırır.
/// </summary>
public class SwaggerServiceConfiguration : IServiceConfiguration
{
    public int Order => 8;

    public IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "ETicaret API", Version = "v1" });

            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Lütfen geçerli bir token girin (başına 'Bearer ' ekleyerek).",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new string[]{}
                }
            });

            opt.OperationFilter<AuthorizeCheckOperationFilter>();
        });

        return services;
    }
}