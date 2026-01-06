using AutoMapper;
using Core.Application.Abstractions.Paging;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Results;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Addresses.Queries.GetList;

[DefaultRoles("Admin")]
public class GetListAddressQuery : IRequest<IPaginate<GetListAddressResponseDto>>, ICachableRequest
{
    #region Properties
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string? UserNameSearch { get; set; }
    public string? CityFilter { get; set; }
    #endregion

    #region Önbellek Ayarları (Cache Settings)
    public bool BypassCache { get; set; }
    public string CacheKey => $"default-shipping-address-list:page_{PageIndex}-size_{PageSize}-user_{UserNameSearch}-city_{CityFilter}";
    public string? CacheGroupKey => "Addresses";
    public TimeSpan? SlidingExpiration { get; set; } = TimeSpan.FromMinutes(10);
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; } = TimeSpan.FromMinutes(30);
    #endregion
}

#region Get List Address Query Handler

public class GetListAddressQueryHandler : IRequestHandler<GetListAddressQuery, IPaginate<GetListAddressResponseDto>>
{
    #region Fields

    private readonly IAddressRepository _addressRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor
    public GetListAddressQueryHandler(IAddressRepository addressRepository, IMapper mapper)
    {
        _addressRepository = addressRepository;
        _mapper = mapper;
    }

    #endregion

    #region Handler Implementation

    public async Task<IPaginate<GetListAddressResponseDto>> Handle(GetListAddressQuery request, CancellationToken cancellationToken)
    {
        var spec = new AddressSpecifications.DefaultShippingPerUser(
            request.PageIndex,
            request.PageSize,
            request.UserNameSearch,
            request.CityFilter
        );

        IPaginate<Address> addressesPaginate = await _addressRepository.GetPaginatedListAsync(spec, cancellationToken);
        var mappedItems = _mapper.Map<List<GetListAddressResponseDto>>(addressesPaginate.Items);

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