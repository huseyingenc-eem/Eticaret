using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Caching;
using ETicaret.Application.Features.OperationClaims.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.OperationClaims.Queries.GetList;

#region Get List OperationClaims Query

/// <summary>
/// Tüm operasyon yetkilerini liste halinde getiren sorgu.
/// ICachableRequest: Bu sorgunun sonucunun önbelleğe alınmasını sağlar.
/// Operasyon yetkileri sık değişmediği için önbellekleme performans açısından yararlıdır.
/// </summary>
public class GetListOperationClaimsQuery : IRequest<List<GetListOperationClaimsResponseDto>>, ICachableRequest
{
    #region Filter Properties

    public string? OperationNameFilter { get; set; }
    public string? FeatureNameFilter { get; set; }
    public string? RequiredRoleFilter { get; set; }

    #endregion

    #region Cache Settings
    public bool BypassCache { get; set; }

    public string CacheKey => $"operation-claims-list_{OperationNameFilter}_{FeatureNameFilter}_{RequiredRoleFilter}";
    public string? CacheGroupKey => "OperationClaims";

    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(30);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(2);

    #endregion
}

#endregion

#region Get List OperationClaims Query Handler

/// <summary>
/// GetListOperationClaimsQuery sorgusunu işleyen handler sınıfı.
/// Filtreleme parametrelerine göre operasyon yetkilerini listeler.
/// </summary>
public class GetListOperationClaimsQueryHandler : IRequestHandler<GetListOperationClaimsQuery, List<GetListOperationClaimsResponseDto>>
{
    #region Fields

    private readonly IRepository<OperationClaim, int> _operationClaimRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor

    /// <summary>
    /// GetListOperationClaimsQueryHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemleri için Unit of Work implementasyonu.</param>
    /// <param name="mapper">AutoMapper servisi.</param>
    public GetListOperationClaimsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _operationClaimRepository = unitOfWork.GetRepository<OperationClaim, int>();
    }

    #endregion

    #region Handler Implementation

    /// <summary>
    /// Operasyon yetkileri listesi getirme sorgusunu işler.
    /// </summary>
    /// <param name="request">Operasyon yetkileri listesi getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Operasyon yetkileri listesini içeren yanıt DTO'su.</returns>
    public async Task<List<GetListOperationClaimsResponseDto>> Handle(GetListOperationClaimsQuery request, CancellationToken cancellationToken)
    {
        // 1. Filtreleme kriterlerine göre uygun specification'ı belirle
        var specification = BuildSpecification(request);

        // 2. Operasyon yetkilerini getir
        List<OperationClaim> operationClaims = await GetOperationClaimsAsync(specification, cancellationToken);

        // 3. Entity'leri DTO'lara dönüştür ve döndür
        return MapToResponseDtos(operationClaims);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// İstek parametrelerine göre uygun specification'ı oluşturur.
    /// </summary>
    /// <param name="request">Sorgu isteği.</param>
    /// <returns>Uygun specification nesnesi.</returns>
    private ISpecification<OperationClaim> BuildSpecification(GetListOperationClaimsQuery request)
    {
        // Eğer hiç filter yoksa tüm kayıtları getir
        if (string.IsNullOrWhiteSpace(request.OperationNameFilter) &&
            string.IsNullOrWhiteSpace(request.FeatureNameFilter) &&
            string.IsNullOrWhiteSpace(request.RequiredRoleFilter))
        {
            return new OperationClaimSpecifications.All();
        }

        // Filtreler varsa öncelik sırasına göre uygun specification'ı seç
        if (!string.IsNullOrWhiteSpace(request.OperationNameFilter))
        {
            return new OperationClaimSpecifications.ByOperationName(request.OperationNameFilter);
        }

        if (!string.IsNullOrWhiteSpace(request.FeatureNameFilter))
        {
            return new OperationClaimSpecifications.ByFeatureName(request.FeatureNameFilter);
        }

        if (!string.IsNullOrWhiteSpace(request.RequiredRoleFilter))
        {
            return new OperationClaimSpecifications.ByRequiredRoles(request.RequiredRoleFilter);
        }

        // Varsayılan olarak tüm kayıtları getir
        return new OperationClaimSpecifications.All();
    }

    /// <summary>
    /// Verilen specification'a göre operasyon yetkilerini getirir.
    /// </summary>
    /// <param name="specification">Sorgu spesifikasyonu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Operasyon yetkileri listesi.</returns>
    private async Task<List<OperationClaim>> GetOperationClaimsAsync(ISpecification<OperationClaim> specification, CancellationToken cancellationToken)
    {
        return await _operationClaimRepository.GetListAsync(specification, cancellationToken);
    }

    /// <summary>
    /// Operasyon yetkisi entity'lerini yanıt DTO'larına dönüştürür.
    /// </summary>
    /// <param name="operationClaims">Dönüştürülecek operasyon yetkisi entity'leri.</param>
    /// <returns>Yanıt DTO'ları listesi.</returns>
    private List<GetListOperationClaimsResponseDto> MapToResponseDtos(List<OperationClaim> operationClaims)
    {
        return _mapper.Map<List<GetListOperationClaimsResponseDto>>(operationClaims);
    }

    #endregion
}

#endregion