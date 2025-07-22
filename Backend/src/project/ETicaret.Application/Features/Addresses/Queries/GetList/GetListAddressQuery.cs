using AutoMapper;
using Core.Application.Abstractions.Paging;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Results;
using ETicaret.Application.Features.Addresses.Specifications; // <-- Yeni spesifikasyonumuzu ekliyoruz
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
    // CacheKey artık daha spesifik ve filtreleri de içeriyor, bu sayede çakışmalar önlenir.
    public string CacheKey => $"address-list:page_{PageIndex}-size_{PageSize}-user_{UserNameSearch}-city_{CityFilter}";
    public string? CacheGroupKey => "Addresses";
    public TimeSpan? SlidingExpiration { get; set; } = TimeSpan.FromMinutes(10);
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; } = TimeSpan.FromMinutes(30);
    #endregion

    /// <summary>
    /// GetListAddressQuery sorgusunu işleyen Handler.
    /// </summary>
    public class GetListAddressQueryHandler : IRequestHandler<GetListAddressQuery, IPaginate<GetListAddressResponseDto>>
    {
        private readonly IRepository<Address, Guid> _addressRepository;
        private readonly IMapper _mapper;

        public GetListAddressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            // Somut IAddressRepository yerine IUnitOfWork üzerinden generic repository'yi alıyoruz.
            _addressRepository = unitOfWork.GetRepository<Address, Guid>();
            _mapper = mapper;
        }

        public async Task<IPaginate<GetListAddressResponseDto>> Handle(GetListAddressQuery request, CancellationToken cancellationToken)
        {
            // 1. Gerekli tüm sorgu mantığını (filtre, sıralama, sayfalama) içeren spesifikasyonu oluşturuyoruz.
            var spec = new PagedAndFilteredAddressesSpecification(
                request.PageIndex,
                request.PageSize,
                request.UserNameSearch,
                request.CityFilter
            );

            // 2. Repository'ye bu spesifikasyonu vererek veritabanından sayfalanmış sonucu çekiyoruz.
            IPaginate<Address> addressesPaginate = await _addressRepository.GetPaginatedListAsync(spec, cancellationToken);

            // 3. Veritabanından gelen entity listesini (Items) DTO listesine map'liyoruz.
            var mappedItems = _mapper.Map<List<GetListAddressResponseDto>>(addressesPaginate.Items);

            // 4. Sonucu, sayfalama bilgilerini koruyarak IPaginate<DTO> formatında döndürüyoruz.
            // Bu, en temiz ve hatasız yöntemdir.
            return new PagedResult<GetListAddressResponseDto>(
                items: mappedItems,
                count: addressesPaginate.Count,
                index: addressesPaginate.Index,
                size: addressesPaginate.Size
            );
        }
    }
}