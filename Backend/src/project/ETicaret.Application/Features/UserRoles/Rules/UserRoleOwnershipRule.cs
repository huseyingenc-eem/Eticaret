using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.UserRoles.Commands.Delete;
using ETicaret.Application.Features.UserRoles.Commands.Update;
using ETicaret.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.UserRoles.Rules;

/// <summary>
/// Kullanıcının belirtilen role sahip olup olmadığını kontrol eder.
/// </summary>
public class UserRoleOwnershipRule :
    IBusinessRule<DeleteUserRoleCommand>,
    IBusinessRule<UpdateUserRolesCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRoleOwnershipRule(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
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
            await CheckUserHasRole(user, role.Name);
        }
    }

    public async Task ExecuteAsync(UpdateUserRolesCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);
        if (user == null) return;

        foreach (var roleId in command.RolesToRemove)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role?.Name != null)
            {
                await CheckUserHasRole(user, role.Name);
            }
        }
    }

    private async Task CheckUserHasRole(User user, string roleName)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        bool roleExists = userRoles.Any(x => string.Equals(x, roleName, StringComparison.OrdinalIgnoreCase));

        if (!roleExists)
        {
            throw new BusinessException(
                message: $"User '{user.UserName}' does not have the role '{roleName}'.",
                userFriendlyMessage: $"Kullanıcının '{roleName}' rolü bulunmuyor.",
                errorCode: "USER_DOES_NOT_HAVE_ROLE"
            );
        }
    }

    public int Priority => 3;
}