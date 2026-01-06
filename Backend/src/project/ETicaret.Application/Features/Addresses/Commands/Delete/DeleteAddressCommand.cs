using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.RequestInfo;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using System.Text.Json.Serialization;

namespace ETicaret.Application.Features.Addresses.Commands.Delete;

#region Delete Address Command


[DefaultRoles("Admin","User")]
public class DeleteAddressCommand : IRequest<DeleteAddressResponseDto>,
    ITransactionalRequest,
    ICacheRemoverRequest,
    IRequestInfoRequest
{
    #region Properties
    public Guid Id { get; set; }

    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;
    #endregion

    #region Cache Settings
    public string CacheKey => $"user-addresses_{UserId}";
    public bool BypassCache => false;
    public string? CacheGroupKey => null;

    #endregion
}

#endregion

#region Delete Address Command Handler

public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, DeleteAddressResponseDto>
{
    #region Fields

    private readonly IAddressRepository _addressRepository;

    #endregion

    #region Constructor


    public DeleteAddressCommandHandler(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    #endregion

    #region Handler Implementation

    /// <summary>
    /// Adres silme komutunu işler.
    /// Business rules RuleEngine tarafından otomatik çalıştırıldı.
    /// </summary>
    /// <param name="request">Adres silme komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Silme işlemi sonucu bilgilerini içeren yanıt DTO'su.</returns>
    public async Task<DeleteAddressResponseDto> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        // 1. Adresi güvenli bir şekilde getir ve yetki kontrolü yap
        Address addressToDelete = await GetAndValidateAddressAsync(request, cancellationToken);

        // 2. Adresi sil
        await DeleteAddressAsync(addressToDelete, cancellationToken);

        return new DeleteAddressResponseDto
        {
            Id = addressToDelete.Id,
            Message = "Adres başarıyla silindi.",
            IsSuccess = true
        };
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Adresi güvenli bir şekilde getirir ve yetki kontrolü yapar.
    /// </summary>
    /// <param name="request">Adres silme komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Silme yetkisi olan adres entity'si.</returns>
    private async Task<Address> GetAndValidateAddressAsync(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        // Güvenlik odaklı specification ile hem ID hem de UserId kontrol et
        var spec = new AddressSpecifications.ById(request.Id);
        Address? addressToDelete = await _addressRepository.GetAsync(spec, cancellationToken);

        if (addressToDelete == null)
        {
            throw new NotFoundException(
                message: $"Address with ID {request.Id} not found or user {request.UserId} does not have permission to delete it.",
                userFriendlyMessage: "Belirtilen adres bulunamadı veya bu adresi silme yetkiniz bulunmuyor.",
                errorCode: "ADDRESS_NOT_FOUND_OR_UNAUTHORIZED"
            );
        }

        return addressToDelete;
    }

    /// <summary>
    /// Adresi veritabanından kalıcı olarak siler.
    /// </summary>
    /// <param name="addressToDelete">Silinecek adres entity'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task DeleteAddressAsync(Address addressToDelete, CancellationToken cancellationToken)
    {
        await _addressRepository.DeleteAsync(addressToDelete, permanent: true, cancellationToken);
    }

    #endregion
}

#endregion