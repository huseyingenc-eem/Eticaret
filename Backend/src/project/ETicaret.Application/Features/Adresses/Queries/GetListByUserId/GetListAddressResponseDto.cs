namespace ETicaret.Application.Features.Addresses.Queries.GetListByUserId;

/// <summary>
/// Kullanıcının adres listesinde gösterilecek temel bilgileri içeren DTO.
/// </summary>
public class GetListAddressResponseDto
{
    /// <summary>
    /// Adres ID'si.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Adres başlığı.
    /// </summary>
    public string AddressTitle { get; set; } = string.Empty;

    /// <summary>
    /// Adresin tam metni (Liste görünümü için kısaltılmış olabilir).
    /// </summary>
    public string FullAddress { get; set; } = string.Empty; // Veya City/District gibi alanlar

    /// <summary>
    /// Fatura adresi mi?
    /// </summary>
    public bool IsBillingAddress { get; set; }

    /// <summary>
    /// Teslimat adresi mi?
    /// </summary>
    public bool IsShippingAddress { get; set; }
}