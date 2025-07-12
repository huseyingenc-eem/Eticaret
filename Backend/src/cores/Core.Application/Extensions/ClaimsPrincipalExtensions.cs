using System.Security.Claims;
using Core.CrossCuttingConcerns.Exceptions;

namespace Core.Application.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var userIdClaim = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) 
        ?? throw new AuthorizationException("User ID claim not found in token.");
        if (Guid.TryParse(userIdClaim, out Guid userId))
        {
            return userId;
        }
        throw new AuthorizationException("Invalid User ID format in token.");
    }

    public static List<string> GetUserRoles(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindAll(ClaimTypes.Role)
                              .Select(r => r.Value)
                              .ToList();
    }

    public static bool IsUserInRole(this ClaimsPrincipal claimsPrincipal, string roleName)
    {
        return claimsPrincipal.IsInRole(roleName);
    }
}