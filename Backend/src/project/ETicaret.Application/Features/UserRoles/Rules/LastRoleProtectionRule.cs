using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.UserRoles.Commands.Delete;
using ETicaret.Application.Features.UserRoles.Commands.Update;
using ETicaret.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.UserRoles.Rules;

/// <summary>
/// Kullanıcının en az bir role sahip kalmasını sağlar.
/// </summary>
public class LastRoleProtectionRule :
    IBusinessRule<DeleteUserRoleCommand>,
    IBusinessRule<UpdateUserRolesCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public LastRoleProtectionRule(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public bool ShouldExecute(DeleteUserRoleCommand command) => true;
    public bool ShouldExecute(UpdateUserRolesCommand command) => command.RolesToRemove.Any();

    public async Task ExecuteAsync(DeleteUserRoleCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);
        var role = await _roleManager.FindByIdAsync(command.RoleId);

        if (user != null && role?.Name != null)
        {
            await CheckUserWillHaveAtLeastOneRole(user, role.Name);
        }
    }

    public async Task ExecuteAsync(UpdateUserRolesCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);
        if (user == null) return;

        var userRoles = await _userManager.GetRolesAsync(user);
        var rolesToRemove = new HashSet<string>();

        // Kaldırılacak rol isimlerini topla
        foreach (var roleId in command.RolesToRemove)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role?.Name != null)
            {
                rolesToRemove.Add(role.Name);
            }
        }

        // Eklenmeyecek ama kaldırılacak rolleri kontrol et
        var remainingRoles = userRoles.Except(rolesToRemove, StringComparer.OrdinalIgnoreCase).Count();
        var addedRoles = command.RolesToAdd.Count;

        if (remainingRoles + addedRoles == 0)
        {
            throw new BusinessException(
                message: $"User '{user.UserName}' must have at least one role. Cannot remove all roles.",
                userFriendlyMessage: "Kullanıcının en az bir rolü olması gerekir. Tüm roller kaldırılamaz.",
                errorCode: "USER_MUST_HAVE_AT_LEAST_ONE_ROLE"
            );
        }
    }

    private async Task CheckUserWillHaveAtLeastOneRole(User user, string roleToRemove)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        if (userRoles.Count <= 1 && userRoles.Contains(roleToRemove, StringComparer.OrdinalIgnoreCase))
        {
            throw new BusinessException(
                message: $"User '{user.UserName}' must have at least one role. Cannot remove the last role '{roleToRemove}'.",
                userFriendlyMessage: "Kullanıcının en az bir rolü olması gerekir. Son rol kaldırılamaz.",
                errorCode: "USER_MUST_HAVE_AT_LEAST_ONE_ROLE"
            );
        }
    }

    public int Priority => 5;
}