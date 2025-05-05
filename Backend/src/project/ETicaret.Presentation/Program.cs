using Core.CrossCuttingConcerns.Logger.Serilog;
using Core.CrossCuttingConcerns.Logger;
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
using Core.CrossCuttingConcerns.Logger;
using Core.CrossCuttingConcerns.Logger.Serilog;
using Core.Application;
using ETicaret.Application.Services.RedisServices;

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
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();

builder.Services.AddSingleton<CachingConfiguration>();
builder.Services.AddSingleton<LoggingConfiguration>();
builder.Services.AddScoped<ILoggerService, FileLogger>();
builder.Services.AddScoped<IRedisService, RedisCasheService>();


builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOptions"));

builder.Services.AddExceptionHandler<HttpExceptionHandler>();
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
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOption.SecurityKey))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
