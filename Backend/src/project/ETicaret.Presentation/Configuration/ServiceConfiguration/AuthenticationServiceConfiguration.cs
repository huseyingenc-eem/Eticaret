using ETicaret.Application.Services.JwtServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ETicaret.Presentation.Configuration.ServiceConfiguration;

/// <summary>
/// JWT Authentication servislerini yapılandırır.
/// Tüm 401 (Unauthorized) senaryolarını (token yok, token geçersiz, token süresi dolmuş)
/// merkezi olarak yönetir.
/// </summary>
public class AuthenticationServiceConfiguration : IServiceConfiguration
{
    public int Order => 7;

    public IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var serviceProvider = services.BuildServiceProvider();
        var tokenOptions = serviceProvider.GetRequiredService<CustomTokenOptions>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            ConfigureTokenValidation(options, tokenOptions);
            ConfigureJwtEvents(options, environment);
        });

        return services;
    }

    private static void ConfigureTokenValidation(JwtBearerOptions options, CustomTokenOptions tokenOptions)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience[0],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    private static void ConfigureJwtEvents(JwtBearerOptions options, IWebHostEnvironment environment)
    {
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                var failure = context.AuthenticateFailure;
                object response;

                if (failure is SecurityTokenExpiredException)
                    response = CreateTokenExpiredResponse(context);

                else if (failure != null)
                    response = CreateInvalidTokenResponse(context, environment);
                else
                    response = CreateChallengeResponse(context);

                await WriteJsonResponse(context.Response, response, environment);
            },
            OnAuthenticationFailed = _ => Task.CompletedTask
        };
    }


    private static object CreateChallengeResponse(JwtBearerChallengeContext context) => new
    {
        type = "urn:ietf:rfc:7235#section-3.1",
        title = "Oturum Gerekli",
        status = 401,
        detail = "Bu sayfayı görüntülemek veya bu işlemi yapmak için giriş yapmalısınız.",
        instance = context.Request.Path.ToString(),
        errorCode = "AUTHENTICATION_REQUIRED",
        userFriendlyMessage = "Lütfen devam etmek için giriş yapın." 
    };

    private static object CreateInvalidTokenResponse(JwtBearerChallengeContext context, IWebHostEnvironment env) => new
    {
        type = "urn:ietf:rfc:7235#section-3.1",
        title = "Geçersiz Oturum",
        status = 401,
        detail = "Oturumunuz doğrulanamadı. Lütfen tekrar giriş yapmayı deneyin.",
        instance = context.Request.Path.ToString(),
        errorCode = "INVALID_TOKEN",
        userFriendlyMessage = "Güvenlik nedeniyle oturumunuz sonlandırıldı. Lütfen tekrar giriş yapın.",
        developerDetail = env.IsDevelopment() ? context.AuthenticateFailure?.ToString() : null
    };

    private static object CreateTokenExpiredResponse(JwtBearerChallengeContext context)
    {
        context.Response.Headers.Append("Token-Expired", "true");
        return new
        {
            type = "urn:ietf:rfc:7235#section-3.1",
            title = "Oturum Süresi Doldu",
            status = 401,
            detail = "Oturumunuzun süresi doldu. Güvenliğiniz için belirli bir süre sonra oturumlar otomatik olarak sonlandırılır.",
            instance = context.Request.Path.ToString(),
            errorCode = "TOKEN_EXPIRED",
            userFriendlyMessage = "Oturum süreniz doldu. Lütfen devam etmek için tekrar giriş yapın."
        };
    }

    /// <summary>
    /// HTTP yanıtına JSON formatında nesne yazmak için ortak metot.
    /// </summary>
    private static async Task WriteJsonResponse(HttpResponse response, object responseObject, IWebHostEnvironment environment)
    {
        response.StatusCode = 401;
        response.ContentType = "application/problem+json";

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = environment.IsDevelopment(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var jsonResponse = JsonSerializer.Serialize(responseObject, jsonOptions);
        await response.WriteAsync(jsonResponse);
    }
}