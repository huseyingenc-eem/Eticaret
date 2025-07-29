namespace Core.Application.Abstractions.Services;
public interface IAuthorizationRuleService
{
    /// <summary>
    /// Verilen operasyon adı için gerekli rolleri döndürür.
    /// </summary>
    /// <param name="operationName">Operasyon adı (örn: "CreateProductCommand")</param>
    /// <returns>Gerekli rollerin listesi</returns>
    Task<string[]> GetRequiredRolesAsync(string operationName);
}