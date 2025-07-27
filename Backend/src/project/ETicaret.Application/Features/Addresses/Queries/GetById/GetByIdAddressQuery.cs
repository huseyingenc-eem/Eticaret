using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.RequestInfo;
using Core.Application.Behaviors.Rules;
using ETicaret.Application.Features.Addresses.Rules;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Addresses.Queries.GetById;

#region Get By Id Address Query

/// <summary>
/// Belirli bir ID'ye sahip adresi getirmek için kullanılan sorgu.
/// Sadece belirli kuralları çalıştırır - RuleConfiguration attribute ile kontrol edilir.
/// ICachableRequest: Bu sorgunun sonucunun önbelleğe alınmasını sağlar.
/// IRequestInfoRequest: Kullanıcı kimliği middleware tarafından otomatik atanır.
/// </summary>
[RuleConfiguration(
    OnlyRules = new[]
    {
        typeof(UserExistsRule)
    }
)]
public class GetByIdAddressQuery : IRequest<GetByIdAddressResponseDto>,
    IRequestInfoRequest,
    ICachableRequest
{
    #region Properties

    /// <summary>
    /// Getirilmek istenen adresin benzersiz kimliği.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// İsteği yapan kullanıcının kimliği. Middleware tarafından JWT token'dan otomatik doldurulur.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    #endregion

    #region Cache Settings

    /// <summary>
    /// Önbelleği atlayıp doğrudan veritabanından veri çekip çekmeyeceğini belirtir.
    /// </summary>
    public bool BypassCache { get; set; }

    /// <summary>
    /// Bu adrese özgü önbellek anahtarı. Güvenlik için UserId de dahil edilir.
    /// </summary>
    public string CacheKey => $"address-detail_{Id}_user_{UserId}";

    /// <summary>
    /// Adresler ile ilgili önbellek grubu anahtarı.
    /// </summary>
    public string? CacheGroupKey => "Addresses";

    /// <summary>
    /// Kayan son kullanma süresi. Adres detayları 30 dakika önbelleklenir.
    /// </summary>
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(30);

    /// <summary>
    /// Mutlak son kullanma süresi. 2 saat sonra kesinlikle yenilenecek.
    /// </summary>
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(2);

    #endregion
}

#endregion

#region Get By Id Address Query Handler

/// <summary>
/// GetByIdAddressQuery sorgusunu işleyen handler sınıfı.
/// Business rules artık RuleEngine tarafından otomatik çalıştırılır.
/// Handler sadece core business logic'e odaklanır.
/// </summary>
public class GetByIdAddressQueryHandler : IRequestHandler<GetByIdAddressQuery, GetByIdAddressResponseDto>
{
    #region Fields

    private readonly IRepository<Address, Guid> _addressRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor

    /// <summary>
    /// GetByIdAddressQueryHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemleri için Unit of Work implementasyonu.</param>
    /// <param name="mapper">AutoMapper servisi.</param>
    public GetByIdAddressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
    }

    #endregion

    #region Handler Implementation

    /// <summary>
    /// Adres getirme sorgusunu işler.
    /// Business rules RuleEngine tarafından otomatik çalıştırıldı.
    /// </summary>
    /// <param name="request">Adres getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Adres bilgilerini içeren yanıt DTO'su.</returns>
    public async Task<GetByIdAddressResponseDto> Handle(GetByIdAddressQuery request, CancellationToken cancellationToken)
    {
        // 1. Adresi güvenli bir şekilde getir
        Address address = await GetAddressSecurelyAsync(request, cancellationToken);

        // 2. Entity'yi DTO'ya dönüştür ve döndür
        return MapToResponseDto(address);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Adresi güvenli bir şekilde getirir ve yetki kontrolü yapar.
    /// Sadece kullanıcının kendi adreslerine erişmesine izin verir.
    /// </summary>
    /// <param name="request">Adres getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Kullanıcının erişim yetkisi olan adres entity'si.</returns>
    private async Task<Address> GetAddressSecurelyAsync(GetByIdAddressQuery request, CancellationToken cancellationToken)
    {
        // Güvenlik odaklı specification ile hem ID hem de UserId kontrol et
        var spec = new AddressSpecifications.ByIdAndUserId(request.Id, request.UserId);
        Address? address = await _addressRepository.GetAsync(spec, cancellationToken);

        if (address == null)
        {
            throw new Core.Application.Common.Exceptions.NotFoundException(
                message: $"Address with ID {request.Id} not found for user {request.UserId} or user does not have permission to access it.",
                userFriendlyMessage: "Belirtilen adres bulunamadı veya bu adrese erişim yetkiniz bulunmuyor.",
                errorCode: "ADDRESS_NOT_FOUND_OR_UNAUTHORIZED"
            );
        }

        return address;
    }

    /// <summary>
    /// Adres entity'sini yanıt DTO'suna dönüştürür.
    /// </summary>
    /// <param name="address">Dönüştürülecek adres entity'si.</param>
    /// <returns>Yanıt DTO'su.</returns>
    private GetByIdAddressResponseDto MapToResponseDto(Address address)
    {
        return _mapper.Map<GetByIdAddressResponseDto>(address);
    }

    #endregion
}

#endregion