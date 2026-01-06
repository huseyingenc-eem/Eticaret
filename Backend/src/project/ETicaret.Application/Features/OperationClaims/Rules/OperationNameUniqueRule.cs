using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.OperationClaims.Commands.Create;
using ETicaret.Application.Features.OperationClaims.Commands.Update;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.OperationClaims.Rules;

/// <summary>
/// Operation name'in benzersiz olmasını kontrol eden tek sorumlu rule
/// </summary>
public class OperationNameUniqueRule :
    IBusinessRule<CreateOperationClaimCommand>,
    IBusinessRule<UpdateOperationClaimCommand>
{
    private readonly IOperationClaimRepository _operationClaimRepository;

    public OperationNameUniqueRule(IOperationClaimRepository operationClaimRepository)
    {
        _operationClaimRepository = operationClaimRepository;
    }

    public bool ShouldExecute(CreateOperationClaimCommand command) => !string.IsNullOrWhiteSpace(command.OperationName);
    public bool ShouldExecute(UpdateOperationClaimCommand command) => !string.IsNullOrWhiteSpace(command.OperationName);

    public async Task ExecuteAsync(CreateOperationClaimCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ByOperationNameSpec(command.OperationName);
        var existingClaim = await _operationClaimRepository.GetAsync(spec, cancellationToken);

        if (existingClaim != null)
        {
            throw new BusinessException(
                message: $"Operation name '{command.OperationName}' already exists.",
                userFriendlyMessage: $"'{command.OperationName}' adlı operasyon zaten mevcut.",
                errorCode: "OPERATION_NAME_NOT_UNIQUE"
            );
        }
    }

    public async Task ExecuteAsync(UpdateOperationClaimCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ByOperationNameExcludingIdSpec(command.OperationName, command.Id);
        var existingClaim = await _operationClaimRepository.GetAsync(spec, cancellationToken);

        if (existingClaim != null)
        {
            throw new BusinessException(
                message: $"Operation name '{command.OperationName}' already exists.",
                userFriendlyMessage: $"'{command.OperationName}' adlı operasyon zaten mevcut.",
                errorCode: "OPERATION_NAME_NOT_UNIQUE"
            );
        }
    }

    public int Priority => 1;

    #region Private Specifications
    private class ByOperationNameSpec : Specification<OperationClaim>
    {
        public ByOperationNameSpec(string operationName)
            : base(oc => oc.OperationName == operationName)
        {
        }
    }

    private class ByOperationNameExcludingIdSpec : Specification<OperationClaim>
    {
        public ByOperationNameExcludingIdSpec(string operationName, int excludeId)
            : base(oc => oc.OperationName == operationName && oc.Id != excludeId)
        {
        }
    }
    #endregion
}