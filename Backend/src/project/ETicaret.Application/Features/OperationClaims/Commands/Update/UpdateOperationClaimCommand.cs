using AutoMapper;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Constants;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.OperationClaims.Specifications;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.OperationClaims.Commands.Update;

#region Update OperationClaim Command

public class UpdateOperationClaimCommand : IRequest<UpdateOperationClaimResponseDto>,
    ITransactionalRequest,
    ICacheRemoverRequest
{
    #region Properties

    public int Id { get; set; }
    public string OperationName { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string RequiredRoles { get; set; } = string.Empty;

    #endregion

    #region Cache Settings
    public string? CacheKey => $"operation-claim:{Id}";
    public bool BypassCache => false;
    public string? CacheGroupKey => "OperationClaims";

    #endregion
}

#endregion

#region Update OperationClaim Command Handler


public class UpdateOperationClaimCommandHandler : IRequestHandler<UpdateOperationClaimCommand, UpdateOperationClaimResponseDto>
{
    #region Fields

    private readonly IOperationClaimRepository _operationClaimRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor

    public UpdateOperationClaimCommandHandler(IOperationClaimRepository operationClaimRepository, IMapper mapper)
    {
        _mapper = mapper;
        _operationClaimRepository = operationClaimRepository;
    }

    #endregion

    #region Handler Implementation

    public async Task<UpdateOperationClaimResponseDto> Handle(UpdateOperationClaimCommand request, CancellationToken cancellationToken)
    {
        OperationClaim existingOperationClaim = await GetAndValidateOperationClaimAsync(request, cancellationToken);

        OperationClaim updatedOperationClaim = await UpdateAndSaveOperationClaimAsync(request, existingOperationClaim, cancellationToken);

        return CreateResponseDto(updatedOperationClaim);
    }

    #endregion

    #region Helper Methods

    private async Task<OperationClaim> GetAndValidateOperationClaimAsync(UpdateOperationClaimCommand request, CancellationToken cancellationToken)
    {
        var spec = new OperationClaimSpecifications.ById(request.Id);
        OperationClaim? existingOperationClaim = await _operationClaimRepository.GetAsync(spec, cancellationToken);

        if (existingOperationClaim == null)
        {
            throw new NotFoundException(
                message: $"OperationClaim with ID {request.Id} not found.",
                userFriendlyMessage: "Belirtilen operasyon yetkisi bulunamadı.",
                errorCode: ApplicationErrorCodes.OperationClaim.NotFound,
                additionalData: new { OperationClaimId = request.Id }
            );
        }

        return existingOperationClaim;
    }

    private async Task<OperationClaim> UpdateAndSaveOperationClaimAsync(UpdateOperationClaimCommand request, OperationClaim existingOperationClaim, CancellationToken cancellationToken)
    {
        _mapper.Map(request, existingOperationClaim);

        existingOperationClaim.SetUpdatedTime(DateTime.UtcNow);

        await _operationClaimRepository.UpdateAsync(existingOperationClaim, cancellationToken);
        return existingOperationClaim;
    }

    private UpdateOperationClaimResponseDto CreateResponseDto(OperationClaim updatedOperationClaim)
    {
        var response = _mapper.Map<UpdateOperationClaimResponseDto>(updatedOperationClaim);
        response.Message = "Operasyon yetkisi başarıyla güncellendi.";
        return response;
    }

    #endregion
}

#endregion