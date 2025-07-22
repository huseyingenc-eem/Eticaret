using AutoMapper;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Addresses.Rules;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Addresses.Queries.GetById;

#region Sorgu Sınıfı (Query Class)

/// <summary>
/// Belirli bir ID'ye sahip adresi getirmek için kullanılan sorgu.
/// IAuthenticatedRequest: Bu isteği yapan kullanıcının kimliğinin (UserId)
/// middleware tarafından otomatik ve güvenli bir şekilde atanmasını sağlar.
/// ICachableRequest: Bu sorgunun sonucunun önbelleğe alınmasını sağlar.
/// </summary>
public class GetByIdAddressQuery : IRequest<GetByIdAddressResponseDto>, IAuthenticatedRequest, ICachableRequest
{
    #region Özellikler (Properties)

    /// <summary>
    /// Getirilmek istenen adresin benzersiz kimliği.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// İsteği yapan kullanıcının kimliği. AssignUserIdMiddleware tarafından JWT token'dan okunarak otomatik olarak doldurulur.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    #endregion

    #region Önbellek Ayarları (Cache Settings)

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

#region Sorgu İşleyici (Query Handler)

/// <summary>
/// GetByIdAddressQuery sorgusunu işleyen handler sınıfı.
/// Güvenlik odaklı tasarım ile kullanıcıların sadece kendi adreslerini görmelerini sağlar.
/// </summary>
public class GetByIdAddressQueryHandler : IRequestHandler<GetByIdAddressQuery, GetByIdAddressResponseDto>
{
    #region Alan Tanımlamaları (Field Declarations)

    private readonly IRepository<Address, Guid> _addressRepository;
    private readonly IMapper _mapper;
    private readonly AddressBusinessRules _addressBusinessRules;

    #endregion

    #region Yapıcı Metot (Constructor)

    /// <summary>
    /// GetByIdAddressQueryHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemleri için Unit of Work deseni implementasyonu.</param>
    /// <param name="mapper">Entity ve DTO dönüşümleri için AutoMapper.</param>
    /// <param name="addressBusinessRules">Adres iş kuralları servisi.</param>
    public GetByIdAddressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, AddressBusinessRules addressBusinessRules)
    {
        _mapper = mapper;
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
        _addressBusinessRules = addressBusinessRules;
    }

    #endregion

    #region İşleme Metotları (Handler Methods)

    /// <summary>
    /// Adres getirme sorgusunu işler.
    /// </summary>
    /// <param name="request">Adres getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Adres bilgilerini içeren yanıt DTO'su.</returns>
    public async Task<GetByIdAddressResponseDto> Handle(GetByIdAddressQuery request, CancellationToken cancellationToken)
    {
        // 1. İstek parametrelerini doğrula
        ValidateRequest(request);

        // 2. İş kuralı kontrollerini gerçekleştir
        await ValidateBusinessRulesAsync(request, cancellationToken);

        // 3. Adresi güvenli bir şekilde getir
        Address address = await GetAddressSecurelyAsync(request, cancellationToken);

        // 4. Entity'yi DTO'ya dönüştür ve döndür
        return MapToResponseDto(address);
    }

    #endregion

    #region Yardımcı Metotlar (Helper Methods)

    /// <summary>
    /// İstek parametrelerinin geçerliliğini kontrol eder.
    /// </summary>
    /// <param name="request">Doğrulanacak istek.</param>
    /// <exception cref="BusinessException">Geçersiz parametre değerleri için fırlatılır.</exception>
    private static void ValidateRequest(GetByIdAddressQuery request)
    {
        if (request.Id == Guid.Empty)
        {
            throw new BusinessException(
                message: "Address ID cannot be empty.",
                userFriendlyMessage: "Geçersiz adres kimliği. Lütfen doğru bir adres seçiniz.",
                errorCode: "INVALID_ADDRESS_ID"
            );
        }

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            throw new BusinessException(
                message: "User ID cannot be empty.",
                userFriendlyMessage: "Kullanıcı kimliği geçersiz. Lütfen tekrar giriş yapınız.",
                errorCode: "INVALID_USER_ID"
            );
        }
    }

    /// <summary>
    /// Adres getirme işlemi öncesi iş kuralı kontrollerini gerçekleştirir.
    /// </summary>
    /// <param name="request">Adres getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task ValidateBusinessRulesAsync(GetByIdAddressQuery request, CancellationToken cancellationToken)
    {
        // İş kuralı 1: Kullanıcı mevcut mu?
        await _addressBusinessRules.CheckUserExistsAsync(request.UserId, cancellationToken);
    }

    /// <summary>
    /// Adresi güvenli bir şekilde getirir ve yetki kontrolü yapar.
    /// Sadece kullanıcının kendi adreslerine erişmesine izin verir.
    /// </summary>
    /// <param name="request">Adres getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Kullanıcının erişim yetkisi olan adres entity'si.</returns>
    /// <exception cref="NotFoundException">Adres bulunamazsa veya yetkisiz erişim durumunda fırlatılır.</exception>
    private async Task<Address> GetAddressSecurelyAsync(GetByIdAddressQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Güvenlik odaklı specification ile hem ID hem de UserId kontrol et
            // Bu yaklaşım, SQL injection saldırılarını önler ve veri güvenliğini sağlar
            var spec = new AddressSpecifications.ByIdAndUserId(request.Id, request.UserId);
            Address? address = await _addressRepository.GetAsync(spec, cancellationToken);

            if (address == null)
            {
                throw new NotFoundException(
                    message: $"Address with ID {request.Id} not found for user {request.UserId} or user does not have permission to access it.",
                    userFriendlyMessage: "Belirtilen adres bulunamadı veya bu adrese erişim yetkiniz bulunmuyor.",
                    errorCode: "ADDRESS_NOT_FOUND_OR_UNAUTHORIZED"
                );
            }

            return address;
        }
        catch (NotFoundException)
        {
            // NotFoundException'ları yeniden fırlat
            throw;
        }
        catch (Exception ex)
        {
            throw new BusinessException(
                message: $"Failed to retrieve address. AddressId: {request.Id}, UserId: {request.UserId}.",
                userFriendlyMessage: "Adres bilgileri alınırken bir hata oluştu. Lütfen tekrar deneyiniz.",
                errorCode: "ADDRESS_RETRIEVAL_FAILED",
                additionalData: new { AddressId = request.Id, UserId = request.UserId, Exception = ex.Message }
            );
        }
    }

    /// <summary>
    /// Adres entity'sini yanıt DTO'suna dönüştürür.
    /// </summary>
    /// <param name="address">Dönüştürülecek adres entity'si.</param>
    /// <returns>Yanıt DTO'su.</returns>
    private GetByIdAddressResponseDto MapToResponseDto(Address address)
    {
        try
        {
            return _mapper.Map<GetByIdAddressResponseDto>(address);
        }
        catch (Exception ex)
        {
            throw new BusinessException(
                message: "Failed to map address entity to response DTO.",
                userFriendlyMessage: "Adres bilgileri işlenirken bir hata oluştu.",
                errorCode: "ADDRESS_MAPPING_FAILED",
                additionalData: new { AddressId = address.Id, Exception = ex.Message }
            );
        }
    }

    #endregion
}

#endregion