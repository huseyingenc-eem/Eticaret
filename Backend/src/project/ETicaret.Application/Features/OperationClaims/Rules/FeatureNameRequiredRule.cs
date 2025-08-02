using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.OperationClaims.Commands.Create;
using ETicaret.Application.Features.OperationClaims.Commands.Update;

namespace ETicaret.Application.Features.OperationClaims.Rules;

/// <summary>
/// Feature name'in boş olmamasını kontrol eden tek sorumlu rule
/// </summary>
public class FeatureNameRequiredRule :
    IBusinessRule<CreateOperationClaimCommand>,
    IBusinessRule<UpdateOperationClaimCommand>
{
    public bool ShouldExecute(CreateOperationClaimCommand command) => true;
    public bool ShouldExecute(UpdateOperationClaimCommand command) => true;

    public Task ExecuteAsync(CreateOperationClaimCommand command, CancellationToken cancellationToken = default)
    {
        ValidateFeatureName(command.FeatureName);
        return Task.CompletedTask;
    }

    public Task ExecuteAsync(UpdateOperationClaimCommand command, CancellationToken cancellationToken = default)
    {
        ValidateFeatureName(command.FeatureName);
        return Task.CompletedTask;
    }

    private static void ValidateFeatureName(string featureName)
    {
        if (string.IsNullOrWhiteSpace(featureName))
        {
            throw new BusinessException(
                message: "Feature name cannot be empty.",
                userFriendlyMessage: "Özellik adı zorunludur.",
                errorCode: "FEATURE_NAME_REQUIRED"
            );
        }
    }

    public int Priority => 0;
}