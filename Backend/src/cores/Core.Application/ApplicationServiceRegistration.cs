using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Logging;
using Core.Application.Behaviors.Performance;
using Core.Application.Behaviors.RequestInfo;
using Core.Application.Behaviors.Rules;
using Core.Application.Behaviors.Transactional;
using Core.Application.Behaviors.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.Application;

/// <summary>
/// Core.Application katmanının temel servislerini DI container'ına ekler.
/// </summary>
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddCoreApplicationServices(this IServiceCollection services)
    {
        #region Rule Engine Services
        // Rule Engine servisleri
        services.AddScoped<IRuleExecutor, RuleExecutor>();
        #endregion
        // MediatR'ı ve temel pipeline davranışlarını ekle
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            // --- GENEL PİPELİNE DAVRANIŞLARI ---
            // Bu sıralama, isteğin işlenme hattını belirler (en dıştan en içe doğru).
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(RequestInfoBehavior<,>));
            configuration.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(BusinessRulesBehavior<,>));
            configuration.AddOpenBehavior(typeof(CacheBehavior<,>));
            configuration.AddOpenBehavior(typeof(CacheRemoveBehavior<,>));
            configuration.AddOpenBehavior(typeof(TransactionBehavior<,>));
            configuration.AddOpenBehavior(typeof(PerformanceBehavior<,>));
        });
        
        // FluentValidation validatörlerini bu assembly için ekle
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}