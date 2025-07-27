using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.RequestInfo;
using Core.Application.Behaviors.Rules;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Addresses.Rules;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Domain.Entities;
using MediatR;
using System.Text.Json.Serialization;
using AutoMapper;

namespace ETicaret.Application.Features.Addresses.Commands.Create;

#region Create Address Command

/// <summary>
/// Yeni bir adres oluşturma işlemini temsil eden komut.
/// Sadece belirli kuralları çalıştırır - RuleConfiguration attribute ile kontrol edilir.
/// FluentValidation: Input validation yapar
/// BusinessRulesValidationBehavior: Business logic kontrolü yapar  
/// Handler: Sadece core business işlemlerini yapar
/// </summary>
[RuleConfiguration(
    OnlyRules = new[]
    {
        typeof(UserExistsRule),
        typeof(AddressLimitRule),
        typeof(DuplicateAddressTitleRule),
        typeof(DefaultAddressTypeRule)
    }
)]
public class CreateAddressCommand : IRequest<CreateAddressResponseDto>,
    ITransactionalRequest,
    ICacheRemoverRequest,
    IRequestInfoRequest
{
    #region Properties

    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
    public string AddressTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
    public bool IsDefaultBilling { get; set; } = false;
    public bool IsDefaultShipping { get; set; } = false;

    #endregion

    #region Cache Settings

    public string? CacheKey => null;
    public bool BypassCache => false;
    public string? CacheGroupKey => "Addresses";

    #endregion
}

#endregion

#region Create Address Command Handler

/// <summary>
/// CreateAddressCommand komutunu işleyen handler sınıfı.
/// Business rules artık RuleEngine tarafından otomatik çalıştırılır.
/// Handler sadece core business logic'e odaklanır.
/// </summary>
public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, CreateAddressResponseDto>
{
    #region Fields

    private readonly IRepository<Address, Guid> _addressRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor

    /// <summary>
    /// CreateAddressCommandHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemleri için Unit of Work implementasyonu.</param>
    /// <param name="mapper">AutoMapper servisi.</param>
    public CreateAddressCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
    }

    #endregion

    #region Handler Implementation

    /// <summary>
    /// Adres oluşturma komutunu işler.
    /// Business rules RuleEngine tarafından otomatik çalıştırıldı.
    /// </summary>
    /// <param name="request">Adres oluşturma komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Oluşturma işlemi sonucu bilgilerini içeren yanıt DTO'su.</returns>
    public async Task<CreateAddressResponseDto> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        // 1. Varsayılan adres işlemlerini gerçekleştir
        await HandleDefaultAddressOperationsAsync(request, cancellationToken);

        // 2. Yeni adresi oluştur ve kaydet
        Address newAddress = await CreateAndSaveAddressAsync(request, cancellationToken);

        // 3. Yanıt DTO'sunu oluştur ve döndür
        return CreateResponseDto(newAddress);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Varsayılan adres işlemlerini gerçekleştirir.
    /// Eğer yeni adres varsayılan olarak işaretlenirse, mevcut varsayılan adresleri günceller.
    /// </summary>
    /// <param name="request">Create address komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task HandleDefaultAddressOperationsAsync(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        if (request.IsDefaultShipping)
        {
            await ClearExistingDefaultAddressAsync(request.UserId, isShipping: true, cancellationToken);
        }

        if (request.IsDefaultBilling)
        {
            await ClearExistingDefaultAddressAsync(request.UserId, isShipping: false, cancellationToken);
        }
    }

    /// <summary>
    /// Mevcut varsayılan adres işaretlerini temizler.
    /// </summary>
    /// <param name="userId">Kullanıcı ID'si.</param>
    /// <param name="isShipping">Kargo adresi mi (true) yoksa fatura adresi mi (false).</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task ClearExistingDefaultAddressAsync(string userId, bool isShipping, CancellationToken cancellationToken)
    {
        var spec = new AddressSpecifications.DefaultAddresses(userId,
            isShipping: isShipping ? true : null,
            isBilling: isShipping ? null : true);

        var existingDefaults = await _addressRepository.GetListAsync(spec, cancellationToken);

        foreach (var address in existingDefaults)
        {
            if (isShipping)
                address.IsDefaultShipping = false;
            else
                address.IsDefaultBilling = false;

            await _addressRepository.UpdateAsync(address, cancellationToken);
        }
    }

    /// <summary>
    /// Yeni adresi oluşturur ve veritabanına kaydeder.
    /// </summary>
    /// <param name="request">Create address komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Oluşturulan adres entity'si.</returns>
    private async Task<Address> CreateAndSaveAddressAsync(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        Address addressEntity = _mapper.Map<Address>(request);
        await _addressRepository.AddAsync(addressEntity, cancellationToken);
        return addressEntity;
    }

    /// <summary>
    /// Yanıt DTO'sunu oluşturur.
    /// </summary>
    /// <param name="addressEntity">Oluşturulan adres entity'si.</param>
    /// <returns>Yanıt DTO'su.</returns>
    private CreateAddressResponseDto CreateResponseDto(Address addressEntity)
    {
        var response = _mapper.Map<CreateAddressResponseDto>(addressEntity);
        response.Message = "Adres başarıyla oluşturuldu.";
        return response;
    }

    #endregion
}

#endregion