using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.UserRoles.Commands.Update;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.UserRoles.Rules;

/// <summary>
/// Bir komut için belirtilen rollerin varlığını kontrol eden iş kuralı.
/// </summary>
public class RoleExistsRule : IBusinessRule<UpdateUserRolesCommand>
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleExistsRule(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public int Priority => 1;

    public bool ShouldExecute(UpdateUserRolesCommand command)
    {
        bool hasRolesToAdd = command.RolesToAdd?.Any(id => !string.IsNullOrWhiteSpace(id)) ?? false;
        bool hasRolesToRemove = command.RolesToRemove?.Any(id => !string.IsNullOrWhiteSpace(id)) ?? false;
        return hasRolesToAdd || hasRolesToRemove;
    }

    public async Task ExecuteAsync(UpdateUserRolesCommand command, CancellationToken cancellationToken = default)
    {
        var allValidRoleIds = (command.RolesToAdd ?? new List<string>())
            .Union(command.RolesToRemove ?? new List<string>())
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct();

        foreach (var roleId in allValidRoleIds)
        {
            await ValidateRoleExists(roleId, cancellationToken);
        }
    }

    private async Task ValidateRoleExists(string roleId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(roleId))
        {
            throw new BusinessException("Role ID cannot be null or empty.", 
                userFriendlyMessage:"Rol ID'si boş olamaz.", 
                errorCode:"INVALID_ROLE_ID");
        }

        IdentityRole? role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            throw new NotFoundException($"Role with ID '{roleId}' was not found.",
                userFriendlyMessage: $"Rol bulunamadı (ID: {roleId}).", 
                errorCode:"ROLE_NOT_FOUND");
        }
    }
}