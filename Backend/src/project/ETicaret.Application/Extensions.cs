using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Caching;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Performance;
using Core.Application.Pipelines.Transactional;
using Core.Application.Pipelines.Validation;
using ETicaret.Application.Features.Categories.Rules;
using ETicaret.Application.Services.Authorization;
using ETicaret.Application.Services.JwtServices;
using ETicaret.Application.Services.RedisServices;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Core.Application.Mappings.Profiles;
using Core.Application.Mappings.Converters;

namespace ETicaret.Application;

public static class Extensions
{

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRedisService, RedisCasheService>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<IJwtService, JwtService>();

        services.AddAutoMapper(config =>
        {
            config.AddMaps(Assembly.GetExecutingAssembly());
            config.AddMaps(typeof(PagingProfile).Assembly);
        });

        // ❗ Buraya eklenmeli
        services.AddTransient(typeof(PaginateTypeConverter<,>));

        services.AddScoped<IOperationClaimSeeder, OperationClaimSeeder>();
        services.AddScoped<CategoryBusinessRules>();

        services.AddOptions();
        services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));

        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            opt.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
            opt.AddOpenBehavior(typeof(AddCachePipeline<,>));
            opt.AddOpenBehavior(typeof(CacheRemovePipeline<,>));
            opt.AddOpenBehavior(typeof(LoggingPipeline<,>));
            opt.AddOpenBehavior(typeof(AuthorizationPipeline<,>));
            opt.AddOpenBehavior(typeof(PerformancePipeline<,>));
            opt.AddOpenBehavior(typeof(TransactionalPipeline<,>));
        });

        return services;
    }
}
