using AutoMapper;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.OperationClaims.Commands.Create;

public class CreateOperationClaimCommand : IRequest<CreateOperationClaimResponseDto>,
    ITransactionalRequest,
    ICacheRemoverRequest
{
    public string OperationName { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string RequiredRoles { get; set; } = string.Empty;

    #region Cache Settings
    public string? CacheKey => null;
    public bool BypassCache => false;
    public string? CacheGroupKey => "OperationClaims";
    #endregion
}

#region Handler


public class CreateOperationClaimCommandHandler : IRequestHandler<CreateOperationClaimCommand, CreateOperationClaimResponseDto>
{
    private readonly IOperationClaimRepository _operationClaimRepository;
    private readonly IMapper _mapper;

    public CreateOperationClaimCommandHandler(IOperationClaimRepository operationClaimRepository, IMapper mapper)
    {
        _operationClaimRepository = operationClaimRepository;
        _mapper = mapper;
    }

    public async Task<CreateOperationClaimResponseDto> Handle(CreateOperationClaimCommand request, CancellationToken cancellationToken)
    {
        var operationClaim = _mapper.Map<OperationClaim>(request);

        await _operationClaimRepository.AddAsync(operationClaim, cancellationToken);

        var response = _mapper.Map<CreateOperationClaimResponseDto>(operationClaim);
        response.Message = "Operasyon yetkisi başarıyla oluşturuldu.";
        return response;
    }
}

#endregion