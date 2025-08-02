using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.RequestInfo;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Domain.Entities;
using MediatR;
using System.Text.Json.Serialization;

namespace ETicaret.Application.Features.Addresses.Queries.GetMyAddresses;

#region Get List By User Id Address Query

/// <summary>
/// Kullanıcının tüm adreslerini getirmek için kullanılan sorgu.
/// ICachableRequest: Bu sorgunun sonucunun önbelleğe alınmasını sağlar.
/// IRequestInfoRequest: Kullanıcı kimliği middleware tarafından otomatik atanır.
/// </summary>
[DefaultRoles("User")]
public class GetMyAddressesQuery : IRequest<List<GetMyAddressesResponseDto>>,
    IRequestInfoRequest,
    ICachableRequest
{
    #region Properties
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;

    #endregion

    #region Cache Settings
    public bool BypassCache { get; set; }
    public string CacheKey => $"user-addresses_{UserId}";
    public string? CacheGroupKey => "Addresses";
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(15);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(1);

    #endregion
}

#endregion

#region Get List By User Id Address Query Handler
public class GetListByUserIdAddressQueryHandler : IRequestHandler<GetMyAddressesQuery, List<GetMyAddressesResponseDto>>
{
    #region Fields

    private readonly IRepository<Address, Guid> _addressRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor
    public GetListByUserIdAddressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
    }

    #endregion

    #region Handler Implementation

    public async Task<List<GetMyAddressesResponseDto>> Handle(GetMyAddressesQuery request, CancellationToken cancellationToken)
    {
        var spec = new AddressSpecifications.UserAddressesOrdered(request.UserId);
        var userAddresses = await _addressRepository.GetListAsync(spec, cancellationToken);

        return _mapper.Map<List<GetMyAddressesResponseDto>>(userAddresses);
    }

    #endregion
}

#endregion