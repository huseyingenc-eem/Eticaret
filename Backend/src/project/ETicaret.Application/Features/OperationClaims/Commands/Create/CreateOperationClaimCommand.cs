using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.OperationClaims.Commands.Create;

/// <summary>
/// Yeni operasyon yetkisi oluşturma komutu
/// </summary>
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

/// <summary>
/// CreateOperationClaimCommand handler
/// Business rules SimpleBusinessRulesBehavior tarafından otomatik çalıştırılır
/// </summary>
public class CreateOperationClaimCommandHandler : IRequestHandler<CreateOperationClaimCommand, CreateOperationClaimResponseDto>
{
    private readonly IRepository<OperationClaim, int> _operationClaimRepository;
    private readonly IMapper _mapper;

    public CreateOperationClaimCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _operationClaimRepository = unitOfWork.GetRepository<OperationClaim, int>();
        _mapper = mapper;
    }

    public async Task<CreateOperationClaimResponseDto> Handle(CreateOperationClaimCommand request, CancellationToken cancellationToken)
    {
        // 1. Rules otomatik çalıştı (uniqueness, required fields, validation)

        // 2. Entity oluştur
        var operationClaim = _mapper.Map<OperationClaim>(request);

        // 3. Kaydet
        await _operationClaimRepository.AddAsync(operationClaim, cancellationToken);

        // 4. Response döndür
        var response = _mapper.Map<CreateOperationClaimResponseDto>(operationClaim);
        response.Message = "Operasyon yetkisi başarıyla oluşturuldu.";
        return response;
    }
}

#endregion