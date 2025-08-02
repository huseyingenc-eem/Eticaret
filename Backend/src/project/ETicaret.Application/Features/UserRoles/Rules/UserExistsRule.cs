using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.UserRoles.Commands.Create;
using ETicaret.Application.Features.UserRoles.Commands.Delete;
using ETicaret.Application.Features.UserRoles.Commands.Update;
using ETicaret.Application.Features.UserRoles.Queries.UserWithRoles;
using ETicaret.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.UserRoles.Rules;

/// <summary>
/// User varlığını kontrol eder.
/// </summary>
public class UserExistsRule :
    IBusinessRule<CreateUserRolesCommand>,
    IBusinessRule<UpdateUserRolesCommand>,
    IBusinessRule<DeleteUserRoleCommand>,
    IBusinessRule<UserWithRolesQuery>
{
    private readonly UserManager<User> _userManager;

    public UserExistsRule(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public bool ShouldExecute(CreateUserRolesCommand command) => !string.IsNullOrWhiteSpace(command.UserId);
    public bool ShouldExecute(UpdateUserRolesCommand command) => !string.IsNullOrWhiteSpace(command.UserId);
    public bool ShouldExecute(DeleteUserRoleCommand command) => !string.IsNullOrWhiteSpace(command.UserId);
    public bool ShouldExecute(UserWithRolesQuery query) => !string.IsNullOrWhiteSpace(query.UserId);

    public async Task ExecuteAsync(CreateUserRolesCommand command, CancellationToken cancellationToken = default)
        => await ValidateUserExists(command.UserId, cancellationToken);

    public async Task ExecuteAsync(UpdateUserRolesCommand command, CancellationToken cancellationToken = default)
        => await ValidateUserExists(command.UserId, cancellationToken);

    public async Task ExecuteAsync(DeleteUserRoleCommand command, CancellationToken cancellationToken = default)
        => await ValidateUserExists(command.UserId, cancellationToken);

    public async Task ExecuteAsync(UserWithRolesQuery query, CancellationToken cancellationToken = default)
        => await ValidateUserExists(query.UserId, cancellationToken);

    private async Task ValidateUserExists(string userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new BusinessException(
                message: "User ID cannot be null or empty.",
                userFriendlyMessage: "Kullanıcı ID'si boş olamaz.",
                errorCode: "INVALID_USER_ID"
            );
        }

        User? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException(
                message: $"User with ID '{userId}' was not found.",
                userFriendlyMessage: $"Kullanıcı bulunamadı (ID: {userId}).",
                errorCode: "USER_NOT_FOUND"
            );
        }
    }

    public int Priority => 0;
}