using AutoMapper;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Addresses.Queries.GetByUserId;

[DefaultRoles("Admin")]
public class GetByUserIdAddressQuery : IRequest<List<GetByUserIdAddressResponseDto>>, ICachableRequest
{
    #region Properties
    public string UserId { get; set; } = string.Empty;
    #endregion

    #region Önbellek Ayarları (Cache Settings)
    public bool BypassCache { get; set; }
    public string CacheKey => $"user-addresses:{UserId}";
    public string? CacheGroupKey => "UserAddresses";
    public TimeSpan? SlidingExpiration { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; } = TimeSpan.FromMinutes(60);
    #endregion
}

#region Get By UserId Address Query Handler

public class GetByUserIdAddressQueryHandler : IRequestHandler<GetByUserIdAddressQuery, List<GetByUserIdAddressResponseDto>>
{
    private readonly IAddressRepository _addressRepository;
    private readonly IMapper _mapper;

    public GetByUserIdAddressQueryHandler(IAddressRepository addressRepository, IMapper mapper)
    {
        _addressRepository = addressRepository;
        _mapper = mapper;
    }

    public async Task<List<GetByUserIdAddressResponseDto>> Handle(GetByUserIdAddressQuery request, CancellationToken cancellationToken)
    {
        var spec = new AddressSpecifications.AllAddressesByUser(request.UserId);

        List<Address> addresses = await _addressRepository.GetListAsync(spec, cancellationToken);

        return _mapper.Map<List<GetByUserIdAddressResponseDto>>(addresses);
    }
}

#endregion