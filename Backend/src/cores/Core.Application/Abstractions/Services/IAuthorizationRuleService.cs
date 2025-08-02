namespace Core.Application.Abstractions.Services;

public interface IAuthorizationRuleService
{
    /// <summary>
    /// Verilen operasyon adı için gerekli rolleri döndürür.
    /// </summary>
    Task<string[]> GetRequiredRolesAsync(string operationName);

    /// <summary>
    /// 🆕 Tüm mevcut policy'leri dinamik olarak döndürür
    /// </summary>
    Task<Dictionary<string, string[]>> GetAllPoliciesAsync();

    /// <summary>
    /// 🆕 Authorization cache'ini temizler
    /// </summary>
    Task ClearAuthorizationCacheAsync();
}