// Core.Application/ApplicationServiceRegistration.cs

using Core.Application.Behaviors.RequestInfo;
// Diğer behavior'larınızın using'leri de buraya gelecek...
// using Core.Application.Behaviors.Validation;
// using Core.Application.Behaviors.Authorization;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            // --- PİPELİNE DAVRANIŞLARINI DOĞRU SIRAYLA BURADA EKLE ---
            // Bu sıralama, yukarıda anlatılan soğan mimarisine uygun olmalıdır.

            // En dış katman: Genel loglama veya performans takibi
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            // Yetkilendirme
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));

            // Doğrulama
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Otomatik bilgi ekleme (Kullanıcı ID, Culture vb.)
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestInfoBehavior<,>));

            // Önbelleğe alma
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

            // Transaction yönetimi (Handler'a en yakın)
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionalBehavior<,>));
        });

        return services;
    }
}