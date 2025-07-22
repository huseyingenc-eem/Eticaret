using AutoMapper;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using ETicaret.Application.Features.Addresses.Specifications; 
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Addresses.Queries.GetListByUserId;

/// <summary>
/// Kimliği doğrulanmış kullanıcının tüm adreslerini getiren sorgu.
/// IAuthenticatedRequest: Bu komuta, isteği yapan kullanıcının kimliğinin (UserId)
/// middleware tarafından otomatik olarak atanmasını sağlar.
/// </summary>
public class GetListByUserIdAddressQuery : IRequest<List<GetListByUserIdAddressResponseDto>>, IAuthenticatedRequest , ICachableRequest
{
    public string UserId { get; set; }
    public string CacheKey => $"Addresses-UserId-{UserId}";
    public bool BypassCache => false;
    public string? CacheGroupKey => "Addresses";
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(30);
    public TimeSpan? AbsoluteExpirationRelativeToNow => null;
}

/// <summary>
/// GetListByUserIdAddressQuery sorgusunu işleyen Handler.
/// </summary>
public class GetListByUserIdAddressQueryHandler : IRequestHandler<GetListByUserIdAddressQuery, List<GetListByUserIdAddressResponseDto>>
{
    private readonly IRepository<Address, Guid> _addressRepository;
    private readonly IMapper _mapper;

    public GetListByUserIdAddressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        // Somut IAddressRepository yerine, IUnitOfWork üzerinden generic repository'yi alıyoruz.
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
    }

    /// <summary>
    /// Kullanıcının adreslerini getirme isteğini işler.
    /// </summary>
    public async Task<List<GetListByUserIdAddressResponseDto>> Handle(GetListByUserIdAddressQuery request, CancellationToken cancellationToken)
    {
        var spec = new AddressSpecifications.ByUserId(request.UserId);

        // 2. Repository'ye bu spesifikasyonu vererek ilgili kullanıcının adreslerini çekiyoruz.
        var addresses = await _addressRepository.GetListAsync(spec, cancellationToken);

        // 3. Gelen entity listesini, cevap olarak döneceğimiz DTO listesine dönüştürüyoruz.
        List<GetListByUserIdAddressResponseDto> response = _mapper.Map<List<GetListByUserIdAddressResponseDto>>(addresses);

        return response;
    }
}