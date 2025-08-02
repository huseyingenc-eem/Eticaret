using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.UserRoles.Commands.Delete;
using ETicaret.Application.Features.UserRoles.Commands.Update;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.UserRoles.Rules;

/// <summary>
/// Kritik rollerin silinmesini engeller.
/// </summary>
public class CriticalRoleProtectionRule :
    IBusinessRule<DeleteUserRoleCommand>,
    IBusinessRule<UpdateUserRolesCommand>
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private static readonly HashSet<string> CriticalRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Admin", "SuperAdmin", "SystemAdmin"
    };

    public CriticalRoleProtectionRule(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public bool ShouldExecute(DeleteUserRoleCommand command) => true;
    public bool ShouldExecute(UpdateUserRolesCommand command) => command.RolesToRemove.Any();

    public async Task ExecuteAsync(DeleteUserRoleCommand command, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(command.RoleId);
        if (role?.Name != null)
        {
            CheckRoleCanBeRemoved(role.Name);
        }
    }

    public async Task ExecuteAsync(UpdateUserRolesCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var roleId in command.RolesToRemove)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role?.Name != null)
            {
                CheckRoleCanBeRemoved(role.Name);
            }
        }
    }

    private static void CheckRoleCanBeRemoved(string roleName)
    {
        if (CriticalRoles.Contains(roleName))
        {
            throw new BusinessException(
                message: $"Critical role '{roleName}' cannot be removed from users.",
                userFriendlyMessage: $"'{roleName}' rolü kritik bir roldür ve kullanıcılardan kaldırılamaz.",
                errorCode: "CRITICAL_ROLE_CANNOT_BE_REMOVED"
            );
        }
    }

    public int Priority => 4;
}