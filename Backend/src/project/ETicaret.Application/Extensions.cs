using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Performance;
using ETicaret.Application.Services.JwtServices;
using ETicaret.Application.Services.RedisServices;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ETicaret.Application;

public static class Extensions
{
    
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IRedisService, RedisCasheService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(opt=>
        {
            opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            opt.AddOpenBehavior(typeof(PerformancePipeline<,>));
            opt.AddOpenBehavior(typeof(AuthorizationPipeline<,>));
        });

        return services;
    }
    public static bool StartsWithA(this string text)
    {
        return text.StartsWith("A");
    }
}
