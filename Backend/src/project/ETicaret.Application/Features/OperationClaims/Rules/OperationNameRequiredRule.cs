using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.OperationClaims.Commands.Create;
using ETicaret.Application.Features.OperationClaims.Commands.Update;

namespace ETicaret.Application.Features.OperationClaims.Rules;

/// <summary>
/// Operation name'in boş olmamasını kontrol eden tek sorumlu rule
/// </summary>
public class OperationNameRequiredRule :
    IBusinessRule<CreateOperationClaimCommand>,
    IBusinessRule<UpdateOperationClaimCommand>
{
    public bool ShouldExecute(CreateOperationClaimCommand command) => true;
    public bool ShouldExecute(UpdateOperationClaimCommand command) => true;

    public Task ExecuteAsync(CreateOperationClaimCommand command, CancellationToken cancellationToken = default)
    {
        ValidateOperationName(command.OperationName);
        return Task.CompletedTask;
    }

    public Task ExecuteAsync(UpdateOperationClaimCommand command, CancellationToken cancellationToken = default)
    {
        ValidateOperationName(command.OperationName);
        return Task.CompletedTask;
    }

    private static void ValidateOperationName(string operationName)
    {
        if (string.IsNullOrWhiteSpace(operationName))
        {
            throw new BusinessException(
                message: "Operation name cannot be empty.",
                userFriendlyMessage: "Operasyon adı zorunludur.",
                errorCode: "OPERATION_NAME_REQUIRED"
            );
        }
    }

    public int Priority => 0;
}