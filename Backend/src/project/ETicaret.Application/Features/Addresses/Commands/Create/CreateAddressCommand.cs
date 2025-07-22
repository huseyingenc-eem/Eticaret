using AutoMapper;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Addresses.Rules;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Addresses.Commands.Create;

#region Komut Sınıfı (Command Class)

/// <summary>
/// Yeni bir adres oluşturma işlemini temsil eden komut.
/// ITransactionalRequest: Bu işlemin bir transaction içinde çalışmasını sağlar.
/// ICacheRemoverRequest: İşlem başarılı olduğunda ilgili önbelleği temizler.
/// IAuthenticatedRequest: Kullanıcı kimliğinin middleware tarafından otomatik atanmasını sağlar.
/// </summary>
public class CreateAddressCommand : IRequest<CreateAddressResponseDto>,
    ITransactionalRequest,
    ICacheRemoverRequest,
    IAuthenticatedRequest
{
    #region Özellikler (Properties)
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

    #region Önbellek Ayarları (Cache Settings)

    /// <summary>
    /// Spesifik önbellek anahtarı. Yeni adres oluşturma için null.
    /// </summary>
    public string? CacheKey => null;

    /// <summary>
    /// Önbellek bypass ayarı.
    /// </summary>
    public bool BypassCache => false;

    /// <summary>
    /// Adresler ile ilgili önbellek grubu anahtarı.
    /// Yeni adres oluşturulduğunda bu gruptaki tüm önbellek verileri temizlenir.
    /// </summary>
    public string? CacheGroupKey => "Addresses";

    #endregion
}

#endregion

#region Komut İşleyici (Command Handler)

/// <summary>
/// CreateAddressCommand komutunu işleyen handler sınıfı.
/// </summary>
public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, CreateAddressResponseDto>
{
    #region Alan Tanımlamaları (Field Declarations)

    private readonly IRepository<Address, Guid> _addressRepository;
    private readonly IMapper _mapper;
    private readonly AddressBusinessRules _addressBusinessRules;

    #endregion

    #region Yapıcı Metot (Constructor)

    /// <summary>
    /// CreateAddressCommandHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemleri için Unit of Work deseni implementasyonu.</param>
    /// <param name="mapper">Entity ve DTO dönüşümleri için AutoMapper.</param>
    /// <param name="addressBusinessRules">Adres iş kuralları servisi.</param>
    public CreateAddressCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, AddressBusinessRules addressBusinessRules)
    {
        _mapper = mapper;
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
        _addressBusinessRules = addressBusinessRules;
    }

    #endregion

    #region İşleme Metotları (Handler Methods)

    /// <summary>
    /// Adres oluşturma komutunu işler.
    /// </summary>
    /// <param name="request">Adres oluşturma komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Oluşturulan adres bilgilerini içeren yanıt DTO'su.</returns>
    public async Task<CreateAddressResponseDto> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        // 1. İş kuralı kontrollerini gerçekleştir
        await ValidateBusinessRulesAsync(request, cancellationToken);

        // 2. Varsayılan adres işlemlerini gerçekleştir
        await HandleDefaultAddressOperationsAsync(request, cancellationToken);

        // 3. Yeni adresi oluştur ve kaydet
        Address newAddress = await CreateAndSaveAddressAsync(request, cancellationToken);

        // 4. Yanıt DTO'sunu oluştur ve döndür
        return CreateResponseDto(newAddress);
    }

    #endregion

    #region Yardımcı Metotlar (Helper Methods)

    /// <summary>
    /// Adres oluşturma işlemi öncesi iş kuralı kontrollerini gerçekleştirir.
    /// </summary>
    /// <param name="request">Adres oluşturma komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task ValidateBusinessRulesAsync(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        // İş kuralı 1: Kullanıcı maksimum adres sayısını aşıyor mu?
        await _addressBusinessRules.CheckUserAddressLimitAsync(request.UserId, cancellationToken);

        // İş kuralı 2: Aynı başlıkta adres var mı?
        await _addressBusinessRules.CheckDuplicateAddressTitleAsync(request.UserId, request.AddressTitle, cancellationToken);

        // İş kuralı 3: En az bir varsayılan adres türü seçilmiş mi?
        _addressBusinessRules.CheckAtLeastOneDefaultAddressType(request.IsDefaultBilling, request.IsDefaultShipping);

        // İş kuralı 4: Kullanıcı mevcut mu?
        await _addressBusinessRules.CheckUserExistsAsync(request.UserId, cancellationToken);
    }

    /// <summary>
    /// Varsayılan adres işlemlerini gerçekleştirir.
    /// </summary>
    /// <param name="request">Adres oluşturma komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task HandleDefaultAddressOperationsAsync(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        // Varsayılan kargo adresi işlemi
        if (request.IsDefaultShipping)
        {
            await ClearExistingDefaultAddressAsync(request.UserId, isShipping: true, cancellationToken);
        }

        // Varsayılan fatura adresi işlemi
        if (request.IsDefaultBilling)
        {
            await ClearExistingDefaultAddressAsync(request.UserId, isShipping: false, cancellationToken);
        }
    }

    /// <summary>
    /// Kullanıcının mevcut varsayılan adreslerini temizler.
    /// </summary>
    /// <param name="userId">Kullanıcının kimliği.</param>
    /// <param name="isShipping">True ise kargo, false ise fatura adresi temizlenir.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task ClearExistingDefaultAddressAsync(string userId, bool isShipping, CancellationToken cancellationToken)
    {
        try
        {
            // Mevcut varsayılan adresleri getir
            var spec = new AddressSpecifications.DefaultAddresses(userId, isShipping: isShipping ? true : null, isBilling: isShipping ? null : true);
            var existingDefaults = await _addressRepository.GetListAsync(spec, cancellationToken);

            // Varsayılan bayrakları kaldır
            foreach (var address in existingDefaults)
            {
                if (isShipping)
                {
                    address.IsDefaultShipping = false;
                }
                else
                {
                    address.IsDefaultBilling = false;
                }
                await _addressRepository.UpdateAsync(address, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            throw new BusinessException(
                message: $"Failed to clear existing default {(isShipping ? "shipping" : "billing")} addresses for user {userId}.",
                userFriendlyMessage: $"Mevcut varsayılan {(isShipping ? "kargo" : "fatura")} adresleri temizlenirken bir hata oluştu.",
                errorCode: "DEFAULT_ADDRESS_CLEAR_FAILED",
                additionalData: new { UserId = userId, IsShipping = isShipping, Exception = ex.Message }
            );
        }
    }

    /// <summary>
    /// Yeni adresi oluşturur ve veritabanına kaydeder.
    /// </summary>
    /// <param name="request">Adres oluşturma komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Oluşturulan adres entity'si.</returns>
    private async Task<Address> CreateAndSaveAddressAsync(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Entity'yi oluştur
            Address addressEntity = _mapper.Map<Address>(request);


            // Veritabanına ekle
            await _addressRepository.AddAsync(addressEntity, cancellationToken);

            // TransactionBehavior otomatik olarak değişiklikleri kaydedecek
            return addressEntity;
        }
        catch (Exception ex)
        {
            throw new BusinessException(
                message: $"Failed to create address for user {request.UserId}.",
                userFriendlyMessage: "Adres oluşturulurken bir hata oluştu. Lütfen tekrar deneyiniz.",
                errorCode: "ADDRESS_CREATION_FAILED",
                additionalData: new { UserId = request.UserId, Exception = ex.Message }
            );
        }
    }

    /// <summary>
    /// Oluşturulan adres entity'sinden yanıt DTO'sunu oluşturur.
    /// </summary>
    /// <param name="addressEntity">Oluşturulan adres entity'si.</param>
    /// <returns>Yanıt DTO'su.</returns>
    private CreateAddressResponseDto CreateResponseDto(Address addressEntity)
    {
        try
        {
            var response = _mapper.Map<CreateAddressResponseDto>(addressEntity);
            response.Message = "Adres başarıyla oluşturuldu.";
            return response;
        }
        catch (Exception ex)
        {
            throw new BusinessException(
                message: "Failed to create response DTO from address entity.",
                userFriendlyMessage: "Yanıt hazırlanırken bir hata oluştu.",
                errorCode: "RESPONSE_MAPPING_FAILED",
                additionalData: new { AddressId = addressEntity.Id, Exception = ex.Message }
            );
        }
    }

    #endregion
}

#endregion