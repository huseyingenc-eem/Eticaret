using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.RequestInfo;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Domain.Entities;
using MediatR;
using System.Text.Json.Serialization;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Behaviors.Authorization;
using ETicaret.Application.Services.Repositories;

namespace ETicaret.Application.Features.Addresses.Commands.Update;

#region Update Address Command

/// <summary>
/// Mevcut bir adresi güncelleme işlemini temsil eden komut.
/// </summary>
[DefaultRoles("Admin","User")]
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
    public string? ZipCode { get; set; }
    public bool IsDefaultBilling { get; set; }
    public bool IsDefaultShipping { get; set; }

    #endregion

    #region Cache Settings

    public string? CacheKey => $"user-addresses_{UserId}";
    public bool BypassCache => false;
    public string? CacheGroupKey => null;

    #endregion
}

#endregion

#region Update Address Command Handler

public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, UpdateAddressResponseDto>
{
    #region Fields

    private readonly IAddressRepository _addressRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor
    public UpdateAddressCommandHandler(IAddressRepository addressRepository, IMapper mapper)
    {
        _mapper = mapper;
        _addressRepository = addressRepository;
    }

    #endregion

    #region Handler Implementation

    public async Task<UpdateAddressResponseDto> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        // 1. Mevcut adresi güvenli bir şekilde getir
        Address existingAddress = await GetAndValidateAddressAsync(request, cancellationToken);

        // 2. Varsayılan adres işlemlerini gerçekleştir
        await HandleDefaultAddressOperationsAsync(request, cancellationToken);

        // 3. Adresi güncelle ve kaydet
        Address updatedAddress = await UpdateAndSaveAddressAsync(request, existingAddress, cancellationToken);

        var response = _mapper.Map<UpdateAddressResponseDto>(updatedAddress);
        response.Message = "Adres başarıyla güncellendi.";
        return response;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Mevcut adresi güvenli bir şekilde getirir ve yetki kontrolü yapar.
    /// </summary>
    private async Task<Address> GetAndValidateAddressAsync(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        // Güvenlik odaklı specification ile hem ID hem de UserId kontrol et
        var spec = new AddressSpecifications.ById(request.Id);
        Address? existingAddress = await _addressRepository.GetAsync(spec, cancellationToken);

        if (existingAddress == null)
        {
            throw new NotFoundException(
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
    /// <param name="excludeAddressId">Hariç tutulacak adres ID'si.</param>
    /// <param name="isShipping">Kargo adresi mi (true) yoksa fatura adresi mi (false).</param>
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
        _mapper.Map(request, existingAddress);
        existingAddress.SetUpdatedTime(DateTime.UtcNow);

        await _addressRepository.UpdateAsync(existingAddress, cancellationToken);
        return existingAddress;
    }

    #endregion
}

#endregion