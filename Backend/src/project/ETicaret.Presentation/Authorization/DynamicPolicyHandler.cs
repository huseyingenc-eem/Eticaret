using Core.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ETicaret.Presentation.Authorization;

/// <summary>
/// Tamamen dinamik policy handler - Hiç deploy gerektirmez!
/// </summary>
public class DynamicPolicyHandler : IAuthorizationHandler
{
    private readonly IAuthorizationRuleService _authorizationRuleService;
    private readonly ILoggerService _loggerService;

    public DynamicPolicyHandler(
        IAuthorizationRuleService authorizationRuleService,
        ILoggerService loggerService)
    {
        _authorizationRuleService = authorizationRuleService;
        _loggerService = loggerService;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        // Tüm requirement'ları kontrol et
        foreach (var requirement in context.Requirements.OfType<DynamicPolicyRequirement>())
        {
            await HandleDynamicPolicyAsync(context, requirement);
        }
    }

    private async Task HandleDynamicPolicyAsync(
        AuthorizationHandlerContext context,
        DynamicPolicyRequirement requirement)
    {
        try
        {
            // Kullanıcının rollerini al
            var userRoles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (!userRoles.Any())
            {
                _loggerService.Warning($"User has no roles for policy: {requirement.PolicyName}");
                return;
            }

            // Veritabanından gerekli rolleri al
            var requiredRoles = await _authorizationRuleService.GetRequiredRolesAsync(requirement.PolicyName);

            if (requiredRoles == null || !requiredRoles.Any())
            {
                // Yetki tanımlanmamışsa izin ver
                context.Succeed(requirement);
                return;
            }

            // Kullanıcının rollerinden herhangi biri gereken rollerde var mı?
            bool hasPermission = requiredRoles.Any(requiredRole =>
                userRoles.Contains(requiredRole, StringComparer.OrdinalIgnoreCase));

            if (hasPermission)
            {
                _loggerService.Info($"Dynamic policy '{requirement.PolicyName}' granted for user with roles: [{string.Join(", ", userRoles)}]");
                context.Succeed(requirement);
            }
            else
            {
                _loggerService.Warning($"Dynamic policy '{requirement.PolicyName}' denied. Required: [{string.Join(", ", requiredRoles)}], User has: [{string.Join(", ", userRoles)}]");
            }
        }
        catch (Exception ex)
        {
            _loggerService.Error($"Error checking dynamic policy '{requirement.PolicyName}': {ex.Message}", ex);
        }
    }
}

/// <summary>
/// Dinamik policy requirement
/// </summary>
public class DynamicPolicyRequirement : IAuthorizationRequirement
{
    public string PolicyName { get; }

    public DynamicPolicyRequirement(string policyName)
    {
        PolicyName = policyName ?? throw new ArgumentNullException(nameof(policyName));
    }
}