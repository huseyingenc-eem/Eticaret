using Core.Application.Abstractions.Repositories;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.Addresses.Rules;

#region Address Business Rules Implementation

/// <summary>
/// Adres entity'si için temel iş kurallarını içeren sınıf.
/// Bu sınıf sadece core business logic'i içerir, individual rule'lar tarafından kullanılır.
/// Repository pattern ve Specification pattern kullanarak veritabanı işlemlerini yönetir.
/// </summary>
public class AddressBusinessRules
{
    #region Constants

    private const int MAX_ADDRESSES_PER_USER = 10; // Kullanıcı başına maksimum adres sayısı

    #endregion

    #region Fields

    private readonly IRepository<Address, Guid> _addressRepository;
    private readonly UserManager<User> _userManager;

    #endregion

    #region Constructor

    /// <summary>
    /// AddressBusinessRules sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemleri için Unit of Work implementasyonu.</param>
    /// <param name="userManager">ASP.NET Identity kullanıcı yöneticisi.</param>
    public AddressBusinessRules(IUnitOfWork unitOfWork, UserManager<User> userManager)
    {
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
        _userManager = userManager;
    }

    #endregion

    #region User Validation Rules

    /// <summary>
    /// Kullanıcının sistemde var olduğunu kontrol eder.
    /// </summary>
    /// <param name="userId">Kontrol edilecek kullanıcı ID'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <exception cref="NotFoundException">Kullanıcı bulunamazsa fırlatılır.</exception>
    public async Task CheckUserExistsAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException(
                message: $"User with ID {userId} was not found.",
                userFriendlyMessage: "Kullanıcı bulunamadı. Lütfen tekrar giriş yapınız.",
                errorCode: "USER_NOT_FOUND"
            );
        }
    }

    #endregion

    #region Address Limit Rules

    /// <summary>
    /// Kullanıcının adres limitini kontrol eder.
    /// </summary>
    /// <param name="userId">Kontrol edilecek kullanıcı ID'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <exception cref="BusinessException">Limit aşılırsa fırlatılır.</exception>
    public async Task CheckUserAddressLimitAsync(string userId, CancellationToken cancellationToken)
    {
        var spec = new AddressSpecifications.ByUserId(userId);
        int userAddressCount = await _addressRepository.CountAsync(spec, cancellationToken);

        if (userAddressCount >= MAX_ADDRESSES_PER_USER)
        {
            throw new BusinessException(
                message: $"User {userId} has reached the maximum address limit of {MAX_ADDRESSES_PER_USER}.",
                userFriendlyMessage: $"Maksimum {MAX_ADDRESSES_PER_USER} adres oluşturabilirsiniz. Yeni adres eklemek için mevcut adreslerden birini siliniz.",
                errorCode: "MAX_ADDRESS_LIMIT_EXCEEDED"
            );
        }
    }

    #endregion

    #region Duplicate Title Rules

    /// <summary>
    /// Aynı kullanıcıda adres başlığının tekrar etmediğini kontrol eder (Create için).
    /// </summary>
    /// <param name="userId">Kullanıcı ID'si.</param>
    /// <param name="addressTitle">Kontrol edilecek adres başlığı.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <exception cref="BusinessException">Tekrar eden başlık varsa fırlatılır.</exception>
    public async Task CheckDuplicateAddressTitleAsync(string userId, string addressTitle, CancellationToken cancellationToken)
    {
        var spec = new AddressSpecifications.ByUserIdAndTitle(userId, addressTitle);
        bool isDuplicate = await _addressRepository.AnyAsync(spec, cancellationToken);

        if (isDuplicate)
        {
            throw new BusinessException(
                message: $"User {userId} already has an address with title '{addressTitle}'.",
                userFriendlyMessage: $"'{addressTitle}' başlığında zaten bir adresiniz mevcut. Lütfen farklı bir başlık seçiniz.",
                errorCode: "DUPLICATE_ADDRESS_TITLE"
            );
        }
    }

    /// <summary>
    /// Aynı kullanıcıda adres başlığının tekrar etmediğini kontrol eder (Update için).
    /// </summary>
    /// <param name="userId">Kullanıcı ID'si.</param>
    /// <param name="addressTitle">Kontrol edilecek adres başlığı.</param>
    /// <param name="excludeAddressId">Kontrol dışında tutulacak adres ID'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <exception cref="BusinessException">Tekrar eden başlık varsa fırlatılır.</exception>
    public async Task CheckDuplicateAddressTitleForUpdateAsync(string userId, string addressTitle, Guid excludeAddressId, CancellationToken cancellationToken)
    {
        var spec = new AddressSpecifications.ByUserIdAndTitleExcludingId(userId, addressTitle, excludeAddressId);
        bool isDuplicate = await _addressRepository.AnyAsync(spec, cancellationToken);

        if (isDuplicate)
        {
            throw new BusinessException(
                message: $"User {userId} already has another address with title '{addressTitle}'.",
                userFriendlyMessage: $"'{addressTitle}' başlığında zaten başka bir adresiniz mevcut. Lütfen farklı bir başlık seçiniz.",
                errorCode: "DUPLICATE_ADDRESS_TITLE_UPDATE"
            );
        }
    }

    #endregion

    #region Default Address Type Rules

    /// <summary>
    /// En az bir varsayılan adres tipinin seçildiğini kontrol eder.
    /// </summary>
    /// <param name="isDefaultBilling">Varsayılan fatura adresi mi.</param>
    /// <param name="isDefaultShipping">Varsayılan kargo adresi mi.</param>
    /// <exception cref="BusinessException">Hiçbiri seçili değilse fırlatılır.</exception>
    public void CheckAtLeastOneDefaultAddressType(bool isDefaultBilling, bool isDefaultShipping)
    {
        if (!isDefaultBilling && !isDefaultShipping)
        {
            throw new BusinessException(
                message: "Address must be marked as either default billing or default shipping.",
                userFriendlyMessage: "Adres en azından varsayılan fatura adresi veya varsayılan kargo adresi olarak işaretlenmelidir.",
                errorCode: "NO_DEFAULT_ADDRESS_TYPE_SELECTED"
            );
        }
    }

    #endregion

    #region Minimum Address Rules

    /// <summary>
    /// Kullanıcının en az bir adresi olması gerektiğini kontrol eder.
    /// </summary>
    /// <param name="userId">Kullanıcı ID'si.</param>
    /// <param name="excludeAddressId">Kontrol dışında tutulacak adres ID'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <exception cref="BusinessException">Son adres silinmeye çalışılırsa fırlatılır.</exception>
    public async Task CheckUserMustHaveAtLeastOneAddressAsync(string userId, Guid excludeAddressId, CancellationToken cancellationToken)
    {
        var spec = new AddressSpecifications.ByUserId(userId);
        var userAddresses = await _addressRepository.GetListAsync(spec, cancellationToken);

        // Silinecek adres hariç kaç adres kalacak?
        var remainingAddressCount = userAddresses.Count(a => a.Id != excludeAddressId);

        if (remainingAddressCount == 0)
        {
            throw new BusinessException(
                message: $"User {userId} must have at least one address.",
                userFriendlyMessage: "En az bir adresiniz olmalıdır. Son adresinizi silemezsiniz.",
                errorCode: "MUST_HAVE_AT_LEAST_ONE_ADDRESS"
            );
        }
    }

    #endregion

    #region Warning Rules

    /// <summary>
    /// Varsayılan adres silinirken kullanıcıya uyarı verir.
    /// Bu metot iş kuralı ihlali yaratmaz, sadece bilgilendirme amaçlıdır.
    /// </summary>
    /// <param name="isDefaultBilling">Varsayılan fatura adresi mi.</param>
    /// <param name="isDefaultShipping">Varsayılan kargo adresi mi.</param>
    public void CheckDefaultAddressDeletionWarning(bool isDefaultBilling, bool isDefaultShipping)
    {
        if (isDefaultBilling || isDefaultShipping)
        {
            string addressType = (isDefaultBilling && isDefaultShipping) ? "fatura ve kargo" :
                                isDefaultBilling ? "fatura" : "kargo";

        }
    }

    #endregion
}

#endregion