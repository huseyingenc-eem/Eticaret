using Core.Infrastructure.Logger.Serilog;
using Core.Infrastructure.Logger;
using ETicaret.Application;
using ETicaret.Application.Services.JwtServices;
using ETicaret.Domain.Entities;
using ETicaret.Persistence;
using ETicaret.Persistence.Contexts;
using ETicaret.Presentation.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ETicaret.Application.Services.RedisServices;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Core.Application.Abstractions.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string ReactCors = "ReactCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(ReactCors, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt => // Mevcut AddSwaggerGen'inizi bu þekilde düzenleyin
{
    // Eðer zaten bir SwaggerDoc tanýmýnýz varsa, o kalabilir:
    // opt.SwaggerDoc("v1", new OpenApiInfo { Title = "ETicaret API", Version = "v1" });

    // JWT Authentication için Swagger yapýlandýrmasý
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Lütfen geçerli bir token girin (baþýna 'Bearer ' ekleyerek). Örnek: \"Bearer {token}\"",
        Name = "Authorization",
        Type = SecuritySchemeType.Http, // HTTP tabanlý kimlik doðrulama
        BearerFormat = "JWT",           // Token formatý JWT
        Scheme = "Bearer"               // Kullanýlan þema "Bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // Yukarýdaki AddSecurityDefinition'daki "Bearer" adýyla eþleþmeli
                }
            },
            new string[]{} // Bu boþ dizi, global olarak tüm endpoint'lere uygulanmasýný saðlar (opsiyonel, sadece [Authorize] olanlara da uygulanabilir)
        }
    });
});
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.Configure<LoggingConfiguration>(builder.Configuration.GetSection("SerilogLogConfigurations"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<LoggingConfiguration>>().Value);

builder.Services.AddScoped<ILoggerService, FileLogger>();
builder.Services.AddScoped<IRedisService, RedisCasheService>();


builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOptions"));

builder.Services.AddStackExchangeRedisCache(opt=>
{
    opt.Configuration = "localhost:6379";
    opt.InstanceName = "ETICARET_CACHE";
});
builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 6;
}).AddEntityFrameworkStores<BaseDBContexts>();

var tokenOption = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOptions>();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
{
    opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidIssuer = tokenOption.Issuer,
        ValidAudience = tokenOption.Audience[0],
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOption.SecurityKey))
    };
});

var app = builder.Build();

// --- OperationClaim Seeding Baþlangýcý ---
// Uygulama baþlarken OperationClaims tablosunu doldurmak için Seeder'ý çalýþtýr.
// using ifadesi scope'un doðru þekilde dispose edilmesini saðlar.
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var seeder = serviceProvider.GetRequiredService<ETicaret.Application.Services.Authorization.IOperationClaimSeeder>();

        await seeder.SeedOperationClaimsAsync();

    }
    catch (Exception ex)
    {
        Console.WriteLine($"OperationClaim seeding sýrasýnda bir hata oluþtu: {ex.ToString()}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<HttpExceptionHandler>();

app.UseHttpsRedirection();

app.UseCors(ReactCors);


app.UseAuthentication();
app.UseAuthorization();


app.UseExceptionHandler(_ => { });
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.MapControllers();

app.Run();
