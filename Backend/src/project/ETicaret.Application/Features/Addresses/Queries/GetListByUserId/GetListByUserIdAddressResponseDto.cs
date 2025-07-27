namespace ETicaret.Application.Features.Addresses.Queries.GetListByUserId;

/// <summary>
/// Kullanıcının adres listesi için yanıt DTO'su.
/// </summary>
public class GetListByUserIdAddressResponseDto
{
    #region Properties

    /// <summary>
    /// Adresin benzersiz kimliği.
    /// </summary>
    public Guid Id { get; set; }

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
}