namespace ETicaret.Application.Features.Addresses.Commands.Create;

public class CreateAddressResponseDto
{
    /// <summary>
    /// Oluşturulan adresin benzersiz kimliği.
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
    /// Cadde/Sokak bilgisi.
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Tam adres metni.
    /// </summary>
    public string AddressLine { get; set; }

    /// <summary>
    /// Posta kodu (opsiyonel).
    /// </summary>
    public string? ZipCode { get; set; }

    /// <summary>
    /// Bu adresin fatura adresi olup olmadığını belirtir.
    /// </summary>
    public bool IsDefaultBilling { get; set; }

    /// <summary>
    /// Bu adresin gönderi adresi olup olmadığını belirtir.
    /// </summary>
    public bool IsDefaultShipping { get; set; }

    /// <summary>
    /// Adresin oluşturulma zamanı.
    /// </summary>
    public DateTime CreatedTime { get; set; }

    /// <summary>
    /// İşlem sonucu hakkında bilgilendirme mesajı.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    public CreateAddressResponseDto()
    {

    }
}
