using AutoMapper;
using Core.Application.Abstractions.Paging;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Results;
using ETicaret.Application.Features.Addresses.Rules;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Addresses.Queries.GetList;

/// <summary>
/// Adresleri filtrelenmiş ve sayfalanmış bir liste olarak getiren sorgu.
/// ICachableRequest: Bu sorgunun sonucunun önbelleğe alınmasını sağlar.
/// </summary>
public class GetListAddressQuery : IRequest<IPaginate<GetListAddressResponseDto>>, ICachableRequest
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;

    // Dinamik filtre parametreleri
    public string? UserNameSearch { get; set; }
    public string? CityFilter { get; set; }

    #region Önbellek Ayarları (Cache Settings)
    public bool BypassCache { get; set; }
    public string CacheKey => $"address-list:page_{PageIndex}-size_{PageSize}-user_{UserNameSearch}-city_{CityFilter}";
    public string? CacheGroupKey => "Addresses";
    public TimeSpan? SlidingExpiration { get; set; } = TimeSpan.FromMinutes(10);
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; } = TimeSpan.FromMinutes(30);
    #endregion
}

#region Get List Address Query Handler

/// <summary>
/// GetListAddressQuery sorgusunu işleyen Handler.
/// Business rules artık RuleEngine tarafından otomatik çalıştırılır.
/// Handler sadece core business logic'e odaklanır.
/// </summary>
public class GetListAddressQueryHandler : IRequestHandler<GetListAddressQuery, IPaginate<GetListAddressResponseDto>>
{
    #region Fields

    private readonly IRepository<Address, Guid> _addressRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor

    /// <summary>
    /// GetListAddressQueryHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemleri için Unit of Work implementasyonu.</param>
    /// <param name="mapper">AutoMapper servisi.</param>
    public GetListAddressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
        _mapper = mapper;
    }

    #endregion

    #region Handler Implementation

    /// <summary>
    /// Adres listesi getirme sorgusunu işler.
    /// Business rules RuleEngine tarafından otomatik çalıştırıldı.
    /// </summary>
    /// <param name="request">Adres listesi getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Sayfalanmış adres listesi.</returns>
    public async Task<IPaginate<GetListAddressResponseDto>> Handle(GetListAddressQuery request, CancellationToken cancellationToken)
    {
        // 1. Mevcut AddressSpecifications.PagedAndFiltered sınıfını kullan
        var spec = new AddressSpecifications.PagedAndFiltered(
            request.PageIndex,
            request.PageSize,
            request.UserNameSearch,
            request.CityFilter
        );

        // 2. Repository'ye bu spesifikasyonu vererek veritabanından sayfalanmış sonucu çek
        IPaginate<Address> addressesPaginate = await _addressRepository.GetPaginatedListAsync(spec, cancellationToken);

        // 3. Veritabanından gelen entity listesini (Items) DTO listesine map'le
        var mappedItems = _mapper.Map<List<GetListAddressResponseDto>>(addressesPaginate.Items);

        // 4. Sonucu, sayfalama bilgilerini koruyarak IPaginate<DTO> formatında döndür
        return new PagedResult<GetListAddressResponseDto>(
            items: mappedItems,
            count: addressesPaginate.Count,
            index: addressesPaginate.Index,
            size: addressesPaginate.Size
        );
    }

    #endregion
}

#endregion