using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.RequestInfo;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Domain.Entities;
using MediatR;
using System.Text.Json.Serialization;

namespace ETicaret.Application.Features.Addresses.Queries.GetById;

#region Get By Id Address Query

/// <summary>
/// Belirli bir ID'ye sahip adresi getirmek için kullanılan sorgu.
/// ICachableRequest: Bu sorgunun sonucunun önbelleğe alınmasını sağlar.
/// IRequestInfoRequest: Kullanıcı kimliği middleware tarafından otomatik atanır.
/// </summary>
public class GetByIdAddressQuery : IRequest<GetByIdAddressResponseDto>,
    IRequestInfoRequest,
    ICachableRequest
{
    #region Properties
    public Guid Id { get; set; }
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;

    #endregion

    #region Cache Settings
    public bool BypassCache { get; set; }
    public string CacheKey => $"address-detail_{Id}_user_{UserId}";
    public string? CacheGroupKey => "Addresses";
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(30);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(2);

    #endregion
}

#endregion

#region Get By Id Address Query Handler
public class GetByIdAddressQueryHandler : IRequestHandler<GetByIdAddressQuery, GetByIdAddressResponseDto>
{
    #region Fields

    private readonly IRepository<Address, Guid> _addressRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor
    public GetByIdAddressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
    }

    #endregion

    #region Handler Implementation

    public async Task<GetByIdAddressResponseDto> Handle(GetByIdAddressQuery request, CancellationToken cancellationToken)
    {
        var spec = new AddressSpecifications.ById(request.Id);
        Address? address = await _addressRepository.GetAsync(spec, cancellationToken);

        if (address == null)
        {
            throw new Core.Application.Common.Exceptions.NotFoundException(
                message: $"Address with ID {request.Id} not found for user {request.UserId} or user does not have permission to access it.",
                userFriendlyMessage: "Belirtilen adres bulunamadı veya bu adrese erişim yetkiniz bulunmuyor.",
                errorCode: "ADDRESS_NOT_FOUND_OR_UNAUTHORIZED"
            );
        }
        return _mapper.Map<GetByIdAddressResponseDto>(address);
    }

    #endregion
}

#endregion