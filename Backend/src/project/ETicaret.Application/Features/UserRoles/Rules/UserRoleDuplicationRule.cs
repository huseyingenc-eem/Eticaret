using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.UserRoles.Commands.Create;
using ETicaret.Application.Features.UserRoles.Commands.Update;
using ETicaret.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.UserRoles.Rules;

/// <summary>
/// Kullanıcının aynı role sahip olmasını engeller.
/// </summary>
public class UserRoleDuplicationRule :
    IBusinessRule<CreateUserRolesCommand>,
    IBusinessRule<UpdateUserRolesCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRoleDuplicationRule(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public bool ShouldExecute(CreateUserRolesCommand command) => true;
    public bool ShouldExecute(UpdateUserRolesCommand command) => command.RolesToAdd.Any();

    public async Task ExecuteAsync(CreateUserRolesCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);
        var role = await _roleManager.FindByIdAsync(command.RoleId);

        if (user != null && role?.Name != null)
        {
            await CheckUserDoesNotHaveRole(user, role.Name);
        }
    }

    public async Task ExecuteAsync(UpdateUserRolesCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);
        if (user == null) return;

        foreach (var roleId in command.RolesToAdd)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role?.Name != null)
            {
                await CheckUserDoesNotHaveRole(user, role.Name);
            }
        }
    }

    private async Task CheckUserDoesNotHaveRole(User user, string roleName)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        bool roleExists = userRoles.Any(x => string.Equals(x, roleName, StringComparison.OrdinalIgnoreCase));

        if (roleExists)
        {
            throw new BusinessException(
                message: $"User '{user.UserName}' already has the role '{roleName}'.",
                userFriendlyMessage: $"Kullanıcının '{roleName}' rolü zaten mevcut.",
                errorCode: "USER_ALREADY_HAS_ROLE"
            );
        }
    }

    public int Priority => 2;
}