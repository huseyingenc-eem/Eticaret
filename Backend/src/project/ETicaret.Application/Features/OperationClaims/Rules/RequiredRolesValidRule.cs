using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.OperationClaims.Commands.Create;
using ETicaret.Application.Features.OperationClaims.Commands.Update;

namespace ETicaret.Application.Features.OperationClaims.Rules;

/// <summary>
/// Required roles'ün geçerli formatta olmasını kontrol eden tek sorumlu rule
/// </summary>
public class RequiredRolesValidRule :
    IBusinessRule<CreateOperationClaimCommand>,
    IBusinessRule<UpdateOperationClaimCommand>
{
    public bool ShouldExecute(CreateOperationClaimCommand command) => true;
    public bool ShouldExecute(UpdateOperationClaimCommand command) => true;

    public Task ExecuteAsync(CreateOperationClaimCommand command, CancellationToken cancellationToken = default)
    {
        ValidateRequiredRoles(command.RequiredRoles);
        return Task.CompletedTask;
    }

    public Task ExecuteAsync(UpdateOperationClaimCommand command, CancellationToken cancellationToken = default)
    {
        ValidateRequiredRoles(command.RequiredRoles);
        return Task.CompletedTask;
    }

    private static void ValidateRequiredRoles(string requiredRoles)
    {
        if (string.IsNullOrWhiteSpace(requiredRoles))
        {
            throw new BusinessException(
                message: "Required roles cannot be empty.",
                userFriendlyMessage: "En az bir rol belirtilmelidir.",
                errorCode: "REQUIRED_ROLES_REQUIRED"
            );
        }

        var roles = requiredRoles.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (!roles.Any())
        {
            throw new BusinessException(
                message: "Required roles must contain at least one valid role.",
                userFriendlyMessage: "En az bir geçerli rol belirtilmelidir.",
                errorCode: "REQUIRED_ROLES_INVALID"
            );
        }

        foreach (var role in roles)
        {
            if (string.IsNullOrWhiteSpace(role.Trim()))
            {
                throw new BusinessException(
                    message: "Required roles cannot contain empty values.",
                    userFriendlyMessage: "Roller listesi boş değerler içeremez.",
                    errorCode: "REQUIRED_ROLES_CONTAINS_EMPTY"
                );
            }
        }
    }

    public int Priority => 2;
}