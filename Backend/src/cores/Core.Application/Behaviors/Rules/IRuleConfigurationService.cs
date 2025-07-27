namespace Core.Application.Behaviors.Rules;

/// <summary>
/// Kural yapılandırmasını yöneten servis arayüzü.
/// </summary>
public interface IRuleConfigurationService
{
    /// <summary>
    /// Belirtilen komut türü için çalıştırılması gereken kuralları döndürür.
    /// </summary>
    /// <typeparam name="TCommand">Komut türü</typeparam>
    /// <returns>Çalıştırılacak kural türlerinin listesi</returns>
    IEnumerable<Type> GetRulesToExecute<TCommand>() where TCommand : class;

    /// <summary>
    /// Belirtilen komut türü için kural konfigürasyonunu önbelleğe alır.
    /// </summary>
    /// <typeparam name="TCommand">Komut türü</typeparam>
    void CacheRuleConfiguration<TCommand>() where TCommand : class;
}