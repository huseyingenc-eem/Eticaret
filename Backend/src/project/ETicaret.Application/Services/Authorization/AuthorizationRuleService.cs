using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Services;
using ETicaret.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace ETicaret.Application.Services.Authorization;

/// <summary>
/// Bir operasyon için gerekli olan yetki rollerini (RequiredRoles) belirleyen ve önbelleğe alan servis.
/// </summary>
public class AuthorizationRuleService : IAuthorizationRuleService
{
    private readonly IRepository<OperationClaim, int> _operationClaimRepository;
    private readonly ICacheService? _cacheService;

    public AuthorizationRuleService(IUnitOfWork unitOfWork, ICacheService? cacheService = null)
    {
        // DÜZELTME: Artık IUnitOfWork üzerinden generic repository'yi alıyoruz.
        _operationClaimRepository = unitOfWork.GetRepository<OperationClaim, int>();
        _cacheService = cacheService;
    }

    /// <summary>
    /// Verilen bir operasyon adı için gerekli rolleri, önce önbellekten,
    /// bulunamazsa veritabanından alarak döndürür.
    /// </summary>
    /// <param name="operationName">Yetki rolleri aranacak operasyonun adı (örn: "CreateProductCommand").</param>
    /// <returns>Gerekli rolleri içeren bir string dizisi.</returns>
    public async Task<string[]> GetRequiredRolesAsync(string operationName)
    {
        string cacheKey = $"AuthRule:{operationName}";

        // 1. Önbellekten rol bilgilerini okumayı dene
        if (_cacheService != null)
        {
            try
            {
                var cachedRoles = await _cacheService.GetDataAsync<string[]>(cacheKey);
                if (cachedRoles != null)
                {
                    return cachedRoles;
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapıp devam etmek daha güvenlidir, sistem durmamalı.
                Console.WriteLine($"Redis cache okuma hatası (GetRequiredRolesAsync) - Operation: {operationName}, Hata: {ex.Message}");
            }
        }

        // 2. Önbellekte yoksa, veritabanından spesifikasyon ile çek
        var spec = new OperationClaimByNameSpecification(operationName);
        var operationClaim = await _operationClaimRepository.GetAsync(spec);

        string[] roles = Array.Empty<string>();
        if (operationClaim != null && !string.IsNullOrWhiteSpace(operationClaim.RequiredRoles))
        {
            roles = operationClaim.RequiredRoles
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(role => role.Trim())
                .ToArray();
        }

        // 3. Bulunan sonucu (boş bile olsa) tekrar önbelleğe yaz
        if (_cacheService != null)
        {
            try
            {
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(2)); // Rollerin sık değişmediğini varsayarak uzun süreli cache
                await _cacheService.AddDataAsync(cacheKey, roles, cacheEntryOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis cache yazma hatası (GetRequiredRolesAsync) - Operation: {operationName}, Hata: {ex.Message}");
            }
        }

        return roles;
    }
}