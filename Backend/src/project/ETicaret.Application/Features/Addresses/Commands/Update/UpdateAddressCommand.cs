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

namespace ETicaret.Application.Features.Addresses.Commands.Update;

#region Update Address Command

/// <summary>
/// Mevcut bir adresi güncelleme işlemini temsil eden komut.
/// Sadece belirli kuralları çalıştırır - RuleConfiguration attribute ile kontrol edilir.
/// FluentValidation: Input validation yapar
/// BusinessRulesValidationBehavior: Business logic kontrolü yapar
/// Handler: Sadece core business işlemlerini yapar
/// </summary>
[RuleConfiguration(
    IncludeRules = new[]
    {
        typeof(UserExistsRule),
        typeof(DefaultAddressTypeRule)
    },
    ExcludeRules = new[]
    {
        typeof(DuplicateAddressTitleRule) // Güncelleme işleminde aynı başlığa izin ver
    }
)]
public class UpdateAddressCommand : IRequest<UpdateAddressResponseDto>,
    ITransactionalRequest,
    ICacheRemoverRequest,
    IRequestInfoRequest
{
    #region Properties
    public Guid Id { get; set; }

    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;
    public string AddressTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public bool IsDefaultBilling { get; set; }
    public bool IsDefaultShipping { get; set; }

    #endregion

    #region Cache Settings

    public string? CacheKey => $"address:{Id}";
    public bool BypassCache => false;
    public string? CacheGroupKey => "Addresses";

    #endregion
}

#endregion

#region Update Address Command Handler

/// <summary>
/// UpdateAddressCommand komutunu işleyen handler sınıfı.
/// Business rules artık RuleEngine tarafından otomatik çalıştırılır.
/// Handler sadece core business logic'e odaklanır.
/// </summary>
public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, UpdateAddressResponseDto>
{
    #region Fields

    private readonly IRepository<Address, Guid> _addressRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor

    /// <summary>
    /// UpdateAddressCommandHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemleri için Unit of Work implementasyonu.</param>
    /// <param name="mapper">AutoMapper servisi.</param>
    public UpdateAddressCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
    }

    #endregion

    #region Handler Implementation

    /// <summary>
    /// Adres güncelleme komutunu işler.
    /// Business rules RuleEngine tarafından otomatik çalıştırıldı.
    /// </summary>
    /// <param name="request">Adres güncelleme komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Güncelleme işlemi sonucu bilgilerini içeren yanıt DTO'su.</returns>
    public async Task<UpdateAddressResponseDto> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        // 1. Mevcut adresi güvenli bir şekilde getir
        Address existingAddress = await GetAndValidateAddressAsync(request, cancellationToken);

        // 2. Varsayılan adres işlemlerini gerçekleştir
        await HandleDefaultAddressOperationsAsync(request, cancellationToken);

        // 3. Adresi güncelle ve kaydet
        Address updatedAddress = await UpdateAndSaveAddressAsync(request, existingAddress, cancellationToken);

        // 4. Yanıt DTO'sunu oluştur ve döndür
        return CreateResponseDto(updatedAddress);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Mevcut adresi güvenli bir şekilde getirir ve yetki kontrolü yapar.
    /// </summary>
    /// <param name="request">Update address komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Güncelleme yetkisi olan adres entity'si.</returns>
    private async Task<Address> GetAndValidateAddressAsync(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        // Güvenlik odaklı specification ile hem ID hem de UserId kontrol et
        var spec = new AddressSpecifications.ByIdAndUserId(request.Id, request.UserId);
        Address? existingAddress = await _addressRepository.GetAsync(spec, cancellationToken);

        if (existingAddress == null)
        {
            throw new Core.Application.Common.Exceptions.NotFoundException(
                message: $"Address with ID {request.Id} not found or user {request.UserId} does not have permission to update it.",
                userFriendlyMessage: "Belirtilen adres bulunamadı veya bu adresi güncelleme yetkiniz bulunmuyor.",
                errorCode: "ADDRESS_NOT_FOUND_OR_UNAUTHORIZED"
            );
        }

        return existingAddress;
    }

    /// <summary>
    /// Varsayılan adres işlemlerini gerçekleştirir.
    /// Eğer güncellenen adres varsayılan olarak işaretlenirse, diğer varsayılan adresleri günceller.
    /// </summary>
    /// <param name="request">Update address komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task HandleDefaultAddressOperationsAsync(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        if (request.IsDefaultShipping)
        {
            await ClearExistingDefaultAddressAsync(request.UserId, request.Id, isShipping: true, cancellationToken);
        }

        if (request.IsDefaultBilling)
        {
            await ClearExistingDefaultAddressAsync(request.UserId, request.Id, isShipping: false, cancellationToken);
        }
    }

    /// <summary>
    /// Mevcut varsayılan adres işaretlerini temizler (güncellenecek adres hariç).
    /// </summary>
    /// <param name="userId">Kullanıcı ID'si.</param>
    /// <param name="excludeAddressId">Hariç tutulacak adres ID'si.</param>
    /// <param name="isShipping">Kargo adresi mi (true) yoksa fatura adresi mi (false).</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task ClearExistingDefaultAddressAsync(string userId, Guid excludeAddressId, bool isShipping, CancellationToken cancellationToken)
    {
        var spec = new AddressSpecifications.DefaultAddresses(userId,
            isShipping: isShipping ? true : null,
            isBilling: isShipping ? null : true);

        var existingDefaults = await _addressRepository.GetListAsync(spec, cancellationToken);

        foreach (var address in existingDefaults.Where(a => a.Id != excludeAddressId))
        {
            if (isShipping)
                address.IsDefaultShipping = false;
            else
                address.IsDefaultBilling = false;

            await _addressRepository.UpdateAsync(address, cancellationToken);
        }
    }

    /// <summary>
    /// Adresi günceller ve veritabanına kaydeder.
    /// </summary>
    /// <param name="request">Update address komutu.</param>
    /// <param name="existingAddress">Mevcut adres entity'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Güncellenmiş adres entity'si.</returns>
    private async Task<Address> UpdateAndSaveAddressAsync(UpdateAddressCommand request, Address existingAddress, CancellationToken cancellationToken)
    {
        // AutoMapper ile güncellenmiş verileri mevcut entity'ye aktar
        _mapper.Map(request, existingAddress);

        // UpdateTime'ı manuel olarak set et
        existingAddress.SetUpdatedTime(DateTime.UtcNow);

        await _addressRepository.UpdateAsync(existingAddress, cancellationToken);
        return existingAddress;
    }

    /// <summary>
    /// Yanıt DTO'sunu oluşturur.
    /// </summary>
    /// <param name="updatedAddress">Güncellenmiş adres entity'si.</param>
    /// <returns>Yanıt DTO'su.</returns>
    private UpdateAddressResponseDto CreateResponseDto(Address updatedAddress)
    {
        var response = _mapper.Map<UpdateAddressResponseDto>(updatedAddress);
        response.Message = "Adres başarıyla güncellendi.";
        return response;
    }

    #endregion
}

#endregion