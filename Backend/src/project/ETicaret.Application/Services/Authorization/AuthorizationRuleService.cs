using ETicaret.Application.Services.Authorization;
using ETicaret.Application.Services.Repositories;
using ETicaret.Application.Services.RedisServices;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace ETicaret.Application.Services.Authorization;

public class AuthorizationRuleService : IAuthorizationRuleService
{
    private readonly IOperationClaimRepository _operationClaimRepository;
    private readonly IRedisService? _redisService;

    public AuthorizationRuleService(IOperationClaimRepository operationClaimRepository, IRedisService? redisService = null)
    {
        _operationClaimRepository = operationClaimRepository ?? throw new ArgumentNullException(nameof(operationClaimRepository));
        _redisService = redisService;
    }

    public async Task<string[]> GetRequiredRolesAsync(string operationName)
    {
        string[] roles = Array.Empty<string>();
        string cacheKey = $"AuthRule:{operationName}";

        if (_redisService != null)
        {
            try
            {
                
                var cachedRoles = await _redisService.GetDataAsync<string[]>(cacheKey);
               
                if (cachedRoles != null)
                {
                    return cachedRoles;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis cache okuma hatası (GetRequiredRolesAsync) - Operation: {operationName}, Key: {cacheKey}, Error: {ex.Message}");
               
            }
        }

        var operationClaim = await _operationClaimRepository.GetAsync(
            filter: oc => oc.OperationName == operationName,
            //include: false,
            enableTracking: false
        );

        if (operationClaim != null && !string.IsNullOrWhiteSpace(operationClaim.RequiredRoles))
        {
            roles = operationClaim.RequiredRoles.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(role => role.Trim())
                                            .ToArray();
        }

        if (_redisService != null)
        {
            try
            {
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(2));
                await _redisService.AddDataAsync(cacheKey, roles, cacheEntryOptions, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis cache yazma hatası (GetRequiredRolesAsync) - Operation: {operationName}, Key: {cacheKey}, Error: {ex.Message}");
            }
        }
        return roles;
    }
}
