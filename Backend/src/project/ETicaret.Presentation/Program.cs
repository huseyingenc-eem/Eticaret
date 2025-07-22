using Core.Application.Abstractions.Services;
using Core.Infrastructure.Extensions;
using ETicaret.Application;
using ETicaret.Application.Services.Authorization;
using ETicaret.Application.Services.JwtServices;
using ETicaret.Persistence;
using ETicaret.Presentation.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

#region Servis Kayýtlarý (Service Registration)
// =================================================================================================
// Bu bölümde, uygulamanýn ihtiyaç duyduðu tüm servisler Dependency Injection (DI)
// konteynerine kaydedilir.
// =================================================================================================

/// <summary>
/// Serilog'u yapýlandýrýr ve .NET'in loglama sistemine entegre eder.
/// Ayarlarý appsettings.json dosyasýndan okur ve loglarý konsola yazar.
/// </summary>
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

/// <summary>
/// appsettings.json dosyasýndaki "TokenOptions" bölümünü CustomTokenOptions sýnýfýna baðlar.
/// Bu sayede IOptions<CustomTokenOptions> aracýlýðýyla ayarlara eriþilebilir.
/// </summary>
builder.Services.Configure<CustomTokenOptions>(configuration.GetSection("TokenOptions"));

/// <summary>
/// Farklý katmanlardaki servis kayýtlarýný merkezi olarak çaðýrýr.
/// Bu yaklaþým, Program.cs dosyasýný temiz tutar ve katmanlarýn kendi baðýmlýlýklarýný
/// yönetmesini saðlar.
/// </summary>
builder.Services
    .AddApplicationServices(configuration)
    .AddInfrastructureServices(configuration)
    .AddPersistenceServices(configuration);

/// <summary>
/// Redis'i daðýtýk önbellekleme (distributed cache) saðlayýcýsý olarak yapýlandýrýr.
/// Bu kayýt, IDistributedCache arayüzünün çözümlenmesini saðlar.
/// </summary>
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnectionString");
    options.InstanceName = "ETicaret_"; // Cache anahtarlarýna karýþýklýðý önlemek için bir ön ek ekler.
});

/// <summary>
/// ASP.NET Core'un temel servislerini kaydeder.
/// </summary>
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

/// <summary>
/// Cross-Origin Resource Sharing (CORS) politikasýný tanýmlar.
/// Belirtilen origin'den (React uygulamasý) gelen isteklere izin verir.
/// </summary>
const string ReactCorsPolicy = "ReactCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(ReactCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Frontend uygulamanýzýn adresi
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

/// <summary>
/// JWT (JSON Web Token) tabanlý kimlik doðrulama (Authentication) mekanizmasýný yapýlandýrýr.
/// </summary>
var tokenOptions = configuration.GetSection("TokenOptions").Get<CustomTokenOptions>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
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
});
/// <summary>
/// Swagger/OpenAPI yapýlandýrmasýný yapar. API dokümantasyonu oluþturur ve
/// Swagger UI'da JWT token ile yetkilendirme desteði ekler.
/// </summary>
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "ETicaret API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Lütfen geçerli bir token girin (baþýna 'Bearer ' ekleyerek).",
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
    // Not: AuthorizeCheckOperationFilter sýnýfýnýn projenizde tanýmlý olmasý gerekir.
    // Bu filtre, Swagger UI'da kilit ikonu gibi görselleþtirmeler saðlar.
    // opt.OperationFilter<AuthorizeCheckOperationFilter>(); 
});

#endregion

var app = builder.Build();

#region Middleware Pipeline Yapýlandýrmasý
// =================================================================================================
// Bu bölümde, HTTP istek pipeline'ýna eklenecek olan middleware'ler sýrasýyla tanýmlanýr.
// Middleware'lerin sýrasý çok önemlidir!
// =================================================================================================

// Geliþtirme ortamýnda Swagger ve Swagger UI'ý etkinleþtir.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 1. Hata Yakalama Middleware'i: Pipeline'ýn en baþýna konur ki tüm hatalarý yakalayabilsin.
app.UseMiddleware<HttpExceptionHandler>();

// 2. HTTPS Yönlendirmesi: Gelen HTTP isteklerini otomatik olarak HTTPS'e yönlendirir.
app.UseHttpsRedirection();

// 3. CORS: Tanýmlanan CORS politikasýný uygular.
app.UseCors(ReactCorsPolicy);

// 4. Kimlik Doðrulama (Authentication): Gelen istekteki token'ý doðrular ve kullanýcýnýn kimliðini belirler.
app.UseAuthentication();

// 5. Yetkilendirme (Authorization): Kimliði belirlenen kullanýcýnýn, istenen kaynaða eriþim yetkisi olup olmadýðýný kontrol eder.
app.UseAuthorization();

// 6. Controller Endpoint'lerini Haritalama: Gelen istekleri ilgili Controller Action'larýna yönlendirir.
app.MapControllers();

#endregion

#region Uygulama Baþlangýç Ýþlemleri (Startup Tasks)
// =================================================================================================
// Uygulama ilk kez çalýþtýrýldýðýnda yapýlmasý gereken tek seferlik iþlemler burada yer alýr.
// =================================================================================================

// Veritabanýndaki OperationClaims tablosunu, uygulamadaki mevcut yetkilerle doldurur.
await SeedOperationClaimsAsync(app);

#endregion

// Uygulamayý çalýþtýrýr.
app.Run();


#region Yardýmcý Metotlar (Helper Methods)

/// <summary>
/// Uygulama baþlarken veritabanýndaki yetkileri (Operation Claims) seeder servisi aracýlýðýyla doldurur.
/// </summary>
/// <param name="host">Uygulama host'u.</param>
async Task SeedOperationClaimsAsync(IHost host)
{
    // Seeder gibi scoped servisleri çalýþtýrmak için geçici bir scope oluþturulur.
    await using var scope = host.Services.CreateAsyncScope();
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var seeder = serviceProvider.GetRequiredService<IOperationClaimSeeder>();
        await seeder.SeedOperationClaimsAsync();
    }
    catch (Exception ex)
    {
        // Baþlangýçta loglama servisi hazýr olmayabilir, bu yüzden Console'a yazmak daha güvenlidir.
        Console.WriteLine($"OperationClaim seeding sýrasýnda bir hata oluþtu: {ex}");
    }
}

#endregion
