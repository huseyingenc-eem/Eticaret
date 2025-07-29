using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Addresses.Queries.GetList;

/// <summary>
/// Admin adres listesi için yanıt DTO'su.
/// Kullanıcı bilgilerini de içerir (User join ile gelir).
/// </summary>
public class GetListAddressResponseDto : IMapFrom<Address>
{
    #region Address Properties

    /// <summary>
    /// Adresin benzersiz kimliği.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Adresin hangi kullanıcıya ait olduğu.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Adres başlığı (Ev, İş vb.).
    /// </summary>
    public string AddressTitle { get; set; } = string.Empty;

    /// <summary>
    /// Ülke bilgisi.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Şehir bilgisi.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// İlçe/Semt bilgisi.
    /// </summary>
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// Tam adres metni.
    /// </summary>
    public string AddressLine { get; set; } = string.Empty;

    /// <summary>
    /// Posta kodu (opsiyonel).
    /// </summary>
    public string? ZipCode { get; set; }

    /// <summary>
    /// Bu adresin varsayılan fatura adresi olup olmadığını belirtir.
    /// </summary>
    public bool IsDefaultBilling { get; set; }

    /// <summary>
    /// Bu adresin varsayılan gönderi adresi olup olmadığını belirtir.
    /// </summary>
    public bool IsDefaultShipping { get; set; }

    /// <summary>
    /// Adresin oluşturulma zamanı.
    /// </summary>
    public DateTime CreatedTime { get; set; }

    #endregion

    #region User Properties (Admin için Join ile gelir)

    /// <summary>
    /// Kullanıcının adı (Admin görünümü için).
    /// </summary>
    public string? UserFirstName { get; set; }

    /// <summary>
    /// Kullanıcının soyadı (Admin görünümü için).
    /// </summary>
    public string? UserLastName { get; set; }

    /// <summary>
    /// Kullanıcının email'i (Admin görünümü için).
    /// </summary>
    public string? UserEmail { get; set; }

    #endregion
}