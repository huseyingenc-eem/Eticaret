using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Addresses.Rules;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Addresses.Commands.Delete;

#region Komut Sınıfı (Command Class)

/// <summary>
/// Kullanıcının kendi adresini silme işlemini temsil eden komut.
/// ITransactionalRequest: Bu işlemin bir transaction içinde çalışmasını sağlar.
/// ICacheRemoverRequest: İşlem başarılı olduğunda ilgili önbelleği temizler.
/// IAuthenticatedRequest: Kullanıcı kimliğinin middleware tarafından otomatik atanmasını sağlar.
/// </summary>
public class DeleteAddressCommand : IRequest<DeleteAddressResponseDto>, ITransactionalRequest, ICacheRemoverRequest, IAuthenticatedRequest
{
    #region Özellikler (Properties)
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;

    #endregion

    #region Önbellek Ayarları (Cache Settings)
    public string CacheKey => $"address:{Id}";
    public string? CacheGroupKey => "Addresses";
    public bool BypassCache { get; set; }

    #endregion
}

#endregion

#region Komut İşleyici (Command Handler)

/// <summary>
/// DeleteAddressCommand komutunu işleyen handler sınıfı.
/// Güvenlik odaklı tasarım ile kullanıcıların sadece kendi adreslerini silmelerini sağlar.
/// </summary>
public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, DeleteAddressResponseDto>
{
    #region Alan Tanımlamaları (Field Declarations)

    private readonly IRepository<Address, Guid> _addressRepository;
    private readonly AddressBusinessRules _addressBusinessRules;

    #endregion

    #region Yapıcı Metot (Constructor)

    /// <summary>
    /// DeleteAddressCommandHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemleri için Unit of Work deseni implementasyonu.</param>
    /// <param name="addressBusinessRules">Adres iş kuralları servisi.</param>
    public DeleteAddressCommandHandler(IUnitOfWork unitOfWork, AddressBusinessRules addressBusinessRules)
    {
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
        _addressBusinessRules = addressBusinessRules;
    }

    #endregion

    #region İşleme Metotları (Handler Methods)

    /// <summary>
    /// Adres silme komutunu işler.
    /// </summary>
    /// <param name="request">Adres silme komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Silme işlemi sonucu bilgilerini içeren yanıt DTO'su.</returns>
    public async Task<DeleteAddressResponseDto> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        // 1. İstek parametrelerini doğrula
        ValidateRequest(request);

        // 2. Adres varlığını ve yetki kontrolünü yap
        Address addressToDelete = await GetAndValidateAddressAsync(request, cancellationToken);

        // 3. İş kuralı kontrollerini gerçekleştir
        await ValidateBusinessRulesAsync(addressToDelete, cancellationToken);

        // 4. Adresi sil
        await DeleteAddressAsync(addressToDelete, cancellationToken);

        // 5. Yanıt DTO'sunu oluştur ve döndür
        return CreateResponseDto(request.Id);
    }

    #endregion

    #region Yardımcı Metotlar (Helper Methods)

    /// <summary>
    /// İstek parametrelerinin geçerliliğini kontrol eder.
    /// </summary>
    /// <param name="request">Doğrulanacak istek.</param>
    /// <exception cref="BusinessException">Geçersiz parametre değerleri için fırlatılır.</exception>
    private static void ValidateRequest(DeleteAddressCommand request)
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
    /// Adresi güvenli bir şekilde getirir ve yetki kontrolü yapar.
    /// </summary>
    /// <param name="request">Adres silme komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Silme yetkisi olan adres entity'si.</returns>
    /// <exception cref="NotFoundException">Adres bulunamazsa veya yetkisiz erişim durumunda fırlatılır.</exception>
    private async Task<Address> GetAndValidateAddressAsync(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Güvenlik odaklı specification ile hem ID hem de UserId kontrol et
            var spec = new AddressSpecifications.ByIdAndUserId(request.Id, request.UserId);
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
        catch (NotFoundException)
        {
            // NotFoundException'ları yeniden fırlat
            throw;
        }
        catch (Exception ex)
        {
            throw new BusinessException(
                message: $"Failed to retrieve address for deletion. AddressId: {request.Id}, UserId: {request.UserId}.",
                userFriendlyMessage: "Adres bilgileri alınırken bir hata oluştu. Lütfen tekrar deneyiniz.",
                errorCode: "ADDRESS_RETRIEVAL_FAILED",
                additionalData: new { AddressId = request.Id, UserId = request.UserId, Exception = ex.Message }
            );
        }
    }

    /// <summary>
    /// Adres silme işlemi öncesi iş kuralı kontrollerini gerçekleştirir.
    /// </summary>
    /// <param name="addressToDelete">Silinecek adres entity'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task ValidateBusinessRulesAsync(Address addressToDelete, CancellationToken cancellationToken)
    {
        // İş kuralı 1: Kullanıcının en az bir adresi kalmalı mı kontrolü
        await _addressBusinessRules.CheckUserMustHaveAtLeastOneAddressAsync(addressToDelete.UserId!, addressToDelete.Id, cancellationToken);

        // İş kuralı 2: Varsayılan adres silinirken uyarı
        _addressBusinessRules.CheckDefaultAddressDeletionWarning(addressToDelete.IsDefaultBilling, addressToDelete.IsDefaultShipping);
    }

    /// <summary>
    /// Adresi veritabanından kalıcı olarak siler.
    /// </summary>
    /// <param name="addressToDelete">Silinecek adres entity'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task DeleteAddressAsync(Address addressToDelete, CancellationToken cancellationToken)
    {
        try
        {
            // Kalıcı silme işlemi (permanent: true)
            // Adres verileri genellikle GDPR uyum nedeniyle kalıcı olarak silinir
            await _addressRepository.DeleteAsync(addressToDelete, permanent: true, cancellationToken);

            // TransactionBehavior otomatik olarak değişiklikleri kaydedecek
        }
        catch (Exception ex)
        {
            throw new BusinessException(
                message: $"Failed to delete address with ID {addressToDelete.Id}.",
                userFriendlyMessage: "Adres silinirken bir hata oluştu. Lütfen tekrar deneyiniz.",
                errorCode: "ADDRESS_DELETION_FAILED",
                additionalData: new { AddressId = addressToDelete.Id, UserId = addressToDelete.UserId, Exception = ex.Message }
            );
        }
    }

    /// <summary>
    /// Silme işlemi sonucunu içeren yanıt DTO'sunu oluşturur.
    /// </summary>
    /// <param name="deletedAddressId">Silinen adresin ID'si.</param>
    /// <returns>Yanıt DTO'su.</returns>
    private static DeleteAddressResponseDto CreateResponseDto(Guid deletedAddressId)
    {
        return new DeleteAddressResponseDto
        {
            Id = deletedAddressId,
            Message = "Adres başarıyla silindi.",
            IsSuccess = true
        };
    }

    #endregion
}

#endregion