using ETicaret.Presentation.Middlewares.ExceptionHandling.Strategies;

namespace ETicaret.Presentation.Extensions;

/// <summary>
/// Exception handling servislerini DI container'a kaydetmek için extension metodlar.
/// </summary>
public static class ExceptionHandlingServiceExtensions
{
    /// <summary>
    /// Exception handling strategy'lerini DI container'a ekler.
    /// ⚠️ Sıralama önemli: Spesifik strategy'ler önce, genel olanlar sonda!
    /// </summary>
    public static IServiceCollection AddExceptionHandlingStrategies(this IServiceCollection services)
    {
        // ✅ Tüm strategy'leri SCOPED olarak kaydet
        // Framework IExceptionHandler'ı Singleton olarak kaydediyor,
        // ama biz strategy'leri Service Locator pattern ile runtime'da çözüyoruz.
        services.AddScoped<IExceptionStrategy, RedisExceptionStrategy>();
        services.AddScoped<IExceptionStrategy, ValidationExceptionStrategy>();
        services.AddScoped<IExceptionStrategy, BusinessExceptionStrategy>();
        services.AddScoped<IExceptionStrategy, NotFoundExceptionStrategy>();
        services.AddScoped<IExceptionStrategy, AuthenticationExceptionStrategy>();
        services.AddScoped<IExceptionStrategy, AuthorizationExceptionStrategy>();
        services.AddScoped<IExceptionStrategy, DependencyInjectionExceptionStrategy>();

        // ⚠️ ÖNEMLİ: InternalServerExceptionStrategy EN SON eklenmelidir
        // Çünkü bu strategy her exception'ı handle eder (CanHandle() her zaman true döner)
        services.AddScoped<IExceptionStrategy, InternalServerExceptionStrategy>();

        return services;
    }
}