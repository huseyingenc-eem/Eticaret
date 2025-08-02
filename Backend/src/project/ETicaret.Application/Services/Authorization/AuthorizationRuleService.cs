using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Services;
using ETicaret.Application.Features.OperationClaims.Specifications;
using ETicaret.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace ETicaret.Application.Services.Authorization
{
    /// <summary>
    /// 🚀 Gelişmiş ve dinamik yetkilendirme kural servisi.
    /// Bu servis, operasyon yetkilerini (policies) ve bu yetkiler için gerekli rolleri veritabanından okur.
    /// Performansı artırmak için sonuçları önbelleğe (cache) alır.
    /// Bu sayede, yeni yetkiler eklemek veya mevcut yetkileri değiştirmek için yeniden deploy gerekmez.
    /// </summary>
    public class AuthorizationRuleService : IAuthorizationRuleService
    {
        private readonly IRepository<OperationClaim, int> _operationClaimRepository;
        private readonly ICacheService _cacheService;
        private readonly ILoggerService _loggerService;

        private static readonly TimeSpan DefaultCacheExpiration = TimeSpan.FromMinutes(30);
        private const string CacheKeyPrefix = "AuthRule";
        private const string PolicyListCacheKey = "AllPolicies";
        public AuthorizationRuleService(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            ILoggerService loggerService)
        {
            _operationClaimRepository = unitOfWork.GetRepository<OperationClaim, int>();
            _cacheService = cacheService;
            _loggerService = loggerService;
        }

        /// <summary>
        /// Belirtilen bir operasyon adı (yetki adı) için gerekli olan rollerin listesini döndürür.
        /// Sonucu önce önbellekte arar, bulamazsa veritabanından okur ve önbelleğe kaydeder.
        /// </summary>
        /// <param name="operationName">Gerekli rollerin öğrenilmek istendiği operasyonun adı (örn: "category.add").</param>
        /// <returns>Gerekli rollerin string dizisi. Eğer rol gerekmiyorsa boş bir dizi döner.</returns>
        public async Task<string[]> GetRequiredRolesAsync(string operationName)
        {
            if (string.IsNullOrWhiteSpace(operationName))
            {
                return Array.Empty<string>();
            }

            string cacheKey = $"{CacheKeyPrefix}:{operationName}";

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
                _loggerService.Warning($"Cache read error for operation '{operationName}': {ex.Message}");
            }

            var spec = new OperationClaimSpecifications.ByOperationName(operationName);
            var operationClaim = await _operationClaimRepository.GetAsync(spec);

            string[] roles = Array.Empty<string>();
            if (operationClaim != null && !string.IsNullOrWhiteSpace(operationClaim.RequiredRoles))
            {
                roles = operationClaim.RequiredRoles
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(role => role.Trim())
                    .ToArray();
            }

            try
            {
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(DefaultCacheExpiration);
                await _cacheService.AddDataAsync(cacheKey, roles, cacheEntryOptions);
            }
            catch (Exception ex)
            {
                _loggerService.Warning($"Cache write error for operation '{operationName}': {ex.Message}");
            }

            return roles;
        }

        /// <summary>
        /// ✅ YENİ: Sistemdeki tüm aktif yetki politikalarını (policy) ve bu politikaların gerektirdiği rolleri döndürür.
        /// Bu metot, uygulama başlarken tüm politikaları ASP.NET Core'a dinamik olarak tanıtmak için kullanılır.
        /// </summary>
        /// <returns>Operasyon adını (key) ve gerekli rolleri (value) içeren bir Dictionary.</returns>
        public async Task<Dictionary<string, string[]>> GetAllPoliciesAsync()
        {
            try
            {
                var cachedPolicies = await _cacheService.GetDataAsync<Dictionary<string, string[]>>(PolicyListCacheKey);
                if (cachedPolicies != null)
                {
                    return cachedPolicies;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Warning($"Cache read error for policies: {ex.Message}");
            }

            var allClaims = await _operationClaimRepository.GetListAsync(
                new OperationClaimSpecifications.All());

            var policies = new Dictionary<string, string[]>();

            foreach (var claim in allClaims)
            {
                if (!string.IsNullOrWhiteSpace(claim.OperationName))
                {
                    var roles = string.IsNullOrWhiteSpace(claim.RequiredRoles)
                        ? Array.Empty<string>()
                        : claim.RequiredRoles
                            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(role => role.Trim())
                            .ToArray();

                    policies[claim.OperationName] = roles;
                }
            }

            try
            {
                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15));
                await _cacheService.AddDataAsync(PolicyListCacheKey, policies, cacheOptions);
            }
            catch (Exception ex)
            {
                _loggerService.Warning($"Cache write error for policies: {ex.Message}");
            }

            _loggerService.Info($"Loaded {policies.Count} dynamic policies from database");
            return policies;
        }

        /// <summary>
        /// Bu metot, bir yetki kuralı (örneğin bir operasyona yeni bir rol eklenmesi) değiştirildiğinde çağrılmalıdır.
        /// Böylece sistemin güncel kuralları yeniden yüklemesi sağlanır.
        /// </summary>
        public async Task ClearAuthorizationCacheAsync()
        {
            try
            {
                await _cacheService.RemoveDataAsync(PolicyListCacheKey);

                _loggerService.Info("Authorization cache cleared successfully");
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Error clearing authorization cache: {ex.Message}", ex);
            }
        }
    }
}