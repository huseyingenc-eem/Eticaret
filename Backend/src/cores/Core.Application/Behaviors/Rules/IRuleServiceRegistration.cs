using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Behaviors.Rules;

/// <summary>
/// Feature-specific rule registration'ları için global interface.
/// Her feature kendi rule'larını bu interface üzerinden kaydedebilir.
/// </summary>
public interface IRuleServiceRegistration
{
    /// <summary>
    /// Bu feature'a ait rule'ları DI container'a kaydeder.
    /// </summary>
    /// <param name="services">Servis koleksiyonu.</param>
    void RegisterRules(IServiceCollection services);

    /// <summary>
    /// Bu feature'ın adı. Loglama ve debugging için kullanılır.
    /// </summary>
    string FeatureName { get; }
}