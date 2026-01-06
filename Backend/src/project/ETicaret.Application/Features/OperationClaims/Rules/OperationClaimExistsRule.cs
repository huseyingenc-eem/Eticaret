using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.OperationClaims.Commands.Update;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.OperationClaims.Rules;

public class OperationClaimExistsRule :
    IBusinessRule<UpdateOperationClaimCommand>
{
    private readonly IOperationClaimRepository _operationClaimRepository;

    public OperationClaimExistsRule(IOperationClaimRepository operationClaimRepository)
    {
        _operationClaimRepository = operationClaimRepository;
    }

    public bool ShouldExecute(UpdateOperationClaimCommand command) => true;
    public async Task ExecuteAsync(UpdateOperationClaimCommand command, CancellationToken cancellationToken = default)
        => await ValidateExists(command.Id, cancellationToken);
    private async Task ValidateExists(int id, CancellationToken cancellationToken)
    {
        var spec = new ByIdSpec(id);
        var operationClaim = await _operationClaimRepository.GetAsync(spec, cancellationToken);

        if (operationClaim == null)
        {
            throw new NotFoundException(
                message: $"OperationClaim with ID {id} not found.",
                userFriendlyMessage: "Operasyon yetkisi bulunamadı.",
                errorCode: "OPERATION_CLAIM_NOT_FOUND"
            );
        }
    }

    public int Priority => 0;

    #region Private Specification
    private class ByIdSpec : Specification<OperationClaim>
    {
        public ByIdSpec(int id)
            : base(oc => oc.Id == id)
        {
        }
    }
    #endregion
}