using Core.Application.Abstractions.Services;
using ETicaret.Application.Features.OperationClaims.Specifications;
using ETicaret.Application.Services.Repositories;

using Microsoft.Extensions.Caching.Distributed;

namespace ETicaret.Application.Services.Authorization;


public class AuthorizationRuleService : IAuthorizationRuleService
{
    private readonly IOperationClaimRepository _operationClaimRepository;
    private readonly ICacheService _cacheService;
    private readonly ILoggerService _loggerService;

    private static readonly TimeSpan DefaultCacheExpiration = TimeSpan.FromMinutes(30);
    private const string CacheKeyPrefix = "AuthRule";
    private const string PolicyListCacheKey = "AllPolicies";
    public AuthorizationRuleService(
        IOperationClaimRepository operationClaimRepository,
        ICacheService cacheService,
        ILoggerService loggerService)
    {
        _operationClaimRepository = operationClaimRepository;
        _cacheService = cacheService;
        _loggerService = loggerService;
    }


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