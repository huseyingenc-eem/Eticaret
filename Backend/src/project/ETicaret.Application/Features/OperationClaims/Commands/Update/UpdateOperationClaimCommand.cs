using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Constants;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.OperationClaims.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.OperationClaims.Commands.Update;

#region Update OperationClaim Command

/// <summary>
/// Mevcut bir operasyon yetkisini güncelleme işlemini temsil eden komut.
/// ITransactionalRequest: Bu komutun bir transaction içinde çalışmasını sağlar.
/// ICacheRemoverRequest: Bu komut başarılı olduğunda ilgili önbelleği otomatik olarak temizler.
/// RuleConfiguration: Sadece belirli kuralları çalıştırır.
/// </summary>
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

/// <summary>
/// UpdateOperationClaimCommand komutunu işleyen handler sınıfı.
/// Business rules artık RuleEngine tarafından otomatik çalıştırılır.
/// Handler sadece core business logic'e odaklanır.
/// </summary>
public class UpdateOperationClaimCommandHandler : IRequestHandler<UpdateOperationClaimCommand, UpdateOperationClaimResponseDto>
{
    #region Fields

    private readonly IRepository<OperationClaim, int> _operationClaimRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor

    /// <summary>
    /// UpdateOperationClaimCommandHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemleri için Unit of Work implementasyonu.</param>
    /// <param name="mapper">AutoMapper servisi.</param>
    public UpdateOperationClaimCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _operationClaimRepository = unitOfWork.GetRepository<OperationClaim, int>();
    }

    #endregion

    #region Handler Implementation

    /// <summary>
    /// Operasyon yetkisi güncelleme komutunu işler.
    /// Business rules RuleEngine tarafından otomatik çalıştırıldı.
    /// </summary>
    /// <param name="request">Operasyon yetkisi güncelleme komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Güncelleme işlemi sonucu bilgilerini içeren yanıt DTO'su.</returns>
    public async Task<UpdateOperationClaimResponseDto> Handle(UpdateOperationClaimCommand request, CancellationToken cancellationToken)
    {
        // 1. Mevcut operasyon yetkisini güvenli bir şekilde getir
        OperationClaim existingOperationClaim = await GetAndValidateOperationClaimAsync(request, cancellationToken);

        // 2. Operasyon yetkisini güncelle ve kaydet
        OperationClaim updatedOperationClaim = await UpdateAndSaveOperationClaimAsync(request, existingOperationClaim, cancellationToken);

        // 3. Yanıt DTO'sunu oluştur ve döndür
        return CreateResponseDto(updatedOperationClaim);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Mevcut operasyon yetkisini güvenli bir şekilde getirir.
    /// </summary>
    /// <param name="request">Update operasyon yetkisi komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Güncelleme için operasyon yetkisi entity'si.</returns>
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

    /// <summary>
    /// Operasyon yetkisini günceller ve veritabanına kaydeder.
    /// </summary>
    /// <param name="request">Update operasyon yetkisi komutu.</param>
    /// <param name="existingOperationClaim">Mevcut operasyon yetkisi entity'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Güncellenmiş operasyon yetkisi entity'si.</returns>
    private async Task<OperationClaim> UpdateAndSaveOperationClaimAsync(UpdateOperationClaimCommand request, OperationClaim existingOperationClaim, CancellationToken cancellationToken)
    {
        // AutoMapper ile güncellenmiş verileri mevcut entity'ye aktar
        _mapper.Map(request, existingOperationClaim);

        // UpdateTime'ı manuel olarak set et
        existingOperationClaim.SetUpdatedTime(DateTime.UtcNow);

        await _operationClaimRepository.UpdateAsync(existingOperationClaim, cancellationToken);
        return existingOperationClaim;
    }

    /// <summary>
    /// Yanıt DTO'sunu oluşturur.
    /// </summary>
    /// <param name="updatedOperationClaim">Güncellenmiş operasyon yetkisi entity'si.</param>
    /// <returns>Yanıt DTO'su.</returns>
    private UpdateOperationClaimResponseDto CreateResponseDto(OperationClaim updatedOperationClaim)
    {
        var response = _mapper.Map<UpdateOperationClaimResponseDto>(updatedOperationClaim);
        response.Message = "Operasyon yetkisi başarıyla güncellendi.";
        return response;
    }

    #endregion
}

#endregion