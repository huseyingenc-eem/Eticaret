using AutoMapper;
using Core.Application.Abstractions.Paging;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Results;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Addresses.Queries.GetByUserId;

/// <summary>
/// Belirli bir kullanıcının tüm adreslerini sayfalanmış olarak getiren sorgu.
/// Frontend'de kullanıcı detay sayfası veya pop-up için kullanılır.
/// </summary>
[DefaultRoles("Admin")]
public class GetByUserIdAddressQuery : IRequest<IPaginate<GetByUserIdAddressResponseDto>>, ICachableRequest
{
    #region Properties
    public string UserId { get; set; } = string.Empty;

    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 20;
    #endregion

    #region Önbellek Ayarları (Cache Settings)
    public bool BypassCache { get; set; }
    public string CacheKey => $"user-addresses:user_{UserId}-page_{PageIndex}-size_{PageSize}";
    public string? CacheGroupKey => "UserAddresses";
    public TimeSpan? SlidingExpiration { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; } = TimeSpan.FromMinutes(60);
    #endregion
}

#region Get By UserId Address Query Handler

public class GetByUserIdAddressQueryHandler : IRequestHandler<GetByUserIdAddressQuery, IPaginate<GetByUserIdAddressResponseDto>>
{
    #region Fields

    private readonly IRepository<Address, Guid> _addressRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor
    public GetByUserIdAddressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
        _mapper = mapper;
    }

    #endregion

    #region Handler Implementation

    public async Task<IPaginate<GetByUserIdAddressResponseDto>> Handle(GetByUserIdAddressQuery request, CancellationToken cancellationToken)
    {
        var spec = new AddressSpecifications.AllAddressesByUser(
            request.UserId,
            request.PageIndex,
            request.PageSize
        );

        IPaginate<Address> addressesPaginate = await _addressRepository.GetPaginatedListAsync(spec, cancellationToken);
        var mappedItems = _mapper.Map<List<GetByUserIdAddressResponseDto>>(addressesPaginate.Items);

        return new PagedResult<GetByUserIdAddressResponseDto>(
            items: mappedItems,
            count: addressesPaginate.Count,
            index: addressesPaginate.Index,
            size: addressesPaginate.Size
        );
    }

    #endregion
}

#endregion