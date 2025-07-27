using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.RequestInfo;
using Core.Application.Behaviors.Rules;
using ETicaret.Application.Features.Addresses.Rules;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Domain.Entities;
using MediatR;
using System.Text.Json.Serialization;

namespace ETicaret.Application.Features.Addresses.Queries.GetListByUserId;

#region Get List By User Id Address Query

/// <summary>
/// Kullanıcının tüm adreslerini getirmek için kullanılan sorgu.
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
public class GetListByUserIdAddressQuery : IRequest<List<GetListByUserIdAddressResponseDto>>,
    IRequestInfoRequest,
    ICachableRequest
{
    #region Properties

    /// <summary>
    /// İsteği yapan kullanıcının kimliği. Middleware tarafından JWT token'dan otomatik doldurulur.
    /// </summary>
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;

    #endregion

    #region Cache Settings

    /// <summary>
    /// Önbelleği atlayıp doğrudan veritabanından veri çekip çekmeyeceğini belirtir.
    /// </summary>
    public bool BypassCache { get; set; }

    /// <summary>
    /// Bu kullanıcıya özgü önbellek anahtarı.
    /// </summary>
    public string CacheKey => $"user-addresses_{UserId}";

    /// <summary>
    /// Adresler ile ilgili önbellek grubu anahtarı.
    /// </summary>
    public string? CacheGroupKey => "Addresses";

    /// <summary>
    /// Kayan son kullanma süresi. Kullanıcı adres listesi 15 dakika önbelleklenir.
    /// </summary>
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(15);

    /// <summary>
    /// Mutlak son kullanma süresi. 1 saat sonra kesinlikle yenilenecek.
    /// </summary>
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(1);

    #endregion
}

#endregion

#region Get List By User Id Address Query Handler

/// <summary>
/// GetListByUserIdAddressQuery sorgusunu işleyen handler sınıfı.
/// Business rules artık RuleEngine tarafından otomatik çalıştırılır.
/// Handler sadece core business logic'e odaklanır.
/// </summary>
public class GetListByUserIdAddressQueryHandler : IRequestHandler<GetListByUserIdAddressQuery, List<GetListByUserIdAddressResponseDto>>
{
    #region Fields

    private readonly IRepository<Address, Guid> _addressRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor

    /// <summary>
    /// GetListByUserIdAddressQueryHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemleri için Unit of Work implementasyonu.</param>
    /// <param name="mapper">AutoMapper servisi.</param>
    public GetListByUserIdAddressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
    }

    #endregion

    #region Handler Implementation

    /// <summary>
    /// Kullanıcının adres listesi getirme sorgusunu işler.
    /// Business rules RuleEngine tarafından otomatik çalıştırıldı.
    /// </summary>
    /// <param name="request">Adres listesi getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Kullanıcının adres listesini içeren yanıt DTO'su.</returns>
    public async Task<List<GetListByUserIdAddressResponseDto>> Handle(GetListByUserIdAddressQuery request, CancellationToken cancellationToken)
    {
        // 1. Kullanıcının adreslerini getir
        List<Address> userAddresses = await GetUserAddressesAsync(request.UserId, cancellationToken);

        // 2. Entity'leri DTO'lara dönüştür ve döndür
        return MapToResponseDtos(userAddresses);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Kullanıcının tüm adreslerini getirir.
    /// </summary>
    /// <param name="userId">Kullanıcı ID'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Kullanıcının adres listesi.</returns>
    private async Task<List<Address>> GetUserAddressesAsync(string userId, CancellationToken cancellationToken)
    {
        var spec = new AddressSpecifications.ByUserId(userId);
        return await _addressRepository.GetListAsync(spec, cancellationToken);
    }

    /// <summary>
    /// Adres entity'lerini yanıt DTO'larına dönüştürür.
    /// </summary>
    /// <param name="addresses">Dönüştürülecek adres entity'leri.</param>
    /// <returns>Yanıt DTO'ları listesi.</returns>
    private List<GetListByUserIdAddressResponseDto> MapToResponseDtos(List<Address> addresses)
    {
        return _mapper.Map<List<GetListByUserIdAddressResponseDto>>(addresses);
    }

    #endregion
}

#endregion