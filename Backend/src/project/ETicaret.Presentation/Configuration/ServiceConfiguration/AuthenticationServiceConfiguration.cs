using ETicaret.Application.Services.JwtServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace ETicaret.Presentation.Configuration.ServiceConfiguration;

/// <summary>
/// JWT Authentication servislerini yapılandırır.
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
                await WriteChallengeErrorResponse(context, environment);
            },
            OnAuthenticationFailed = async context =>
            {
                context.NoResult();
                await WriteAuthenticationFailedResponse(context, environment);
            }
        };
    }

    /// <summary>
    /// OnChallenge event'i için error response yazar (JwtBearerChallengeContext).
    /// </summary>
    private static async Task WriteChallengeErrorResponse(
        JwtBearerChallengeContext context,
        IWebHostEnvironment environment)
    {
        var response = new
        {
            type = "urn:ietf:rfc:7235#section-3.1",
            title = "Kimlik Doğrulama Gerekli",
            status = 401,
            detail = "Bu kaynağa erişmek için geçerli bir JWT token gereklidir.",
            instance = context.Request.Path.ToString(),
            errorCode = "AUTHENTICATION_REQUIRED",
            userFriendlyMessage = "Oturum açmanız gerekiyor. Lütfen giriş yapınız."
        };

        await WriteJsonResponse(context.Response, response, environment);
    }

    /// <summary>
    /// OnAuthenticationFailed event'i için error response yazar (AuthenticationFailedContext).
    /// </summary>
    private static async Task WriteAuthenticationFailedResponse(
        AuthenticationFailedContext context,
        IWebHostEnvironment environment)
    {
        var response = new
        {
            type = "urn:ietf:rfc:7235#section-3.1",
            title = "Kimlik Doğrulama Başarısız",
            status = 401,
            detail = "Sağlanan JWT token geçersiz veya süresi dolmuş.",
            instance = context.Request.Path.ToString(),
            errorCode = "INVALID_TOKEN",
            userFriendlyMessage = "Oturumunuzun süresi dolmuş. Lütfen tekrar giriş yapınız.",
            developerDetail = environment.IsDevelopment() ? context.Exception?.Message : null
        };

        await WriteJsonResponse(context.Response, response, environment);
    }

    /// <summary>
    /// JSON response yazmak için ortak method.
    /// </summary>
    private static async Task WriteJsonResponse(
        HttpResponse response,
        object responseObject,
        IWebHostEnvironment environment)
    {
        response.StatusCode = 401;
        response.ContentType = "application/json";
        response.Headers.Append("WWW-Authenticate", "Bearer");

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = environment.IsDevelopment()
        };

        var jsonResponse = JsonSerializer.Serialize(responseObject, jsonOptions);
        await response.WriteAsync(jsonResponse);
    }
}