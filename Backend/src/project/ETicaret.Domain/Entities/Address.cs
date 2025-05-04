using Core.Persistence.Entities;

namespace ETicaret.Domain.Entities;

/// <summary>
/// Kullanıcının teslimat veya fatura adres bilgilerini temsil eder.
/// </summary>
public class Address : Entity<int>
{
    /// <summary>
    /// Adresin ait olduğu kullanıcının kimliği (User tablosuna foreign key).
    /// </summary>
    // [ForeignKey(nameof(User))] // İsteğe bağlı: İlişkiyi attribute ile de belirtebilirsiniz. Configuration sınıfında tanımlamak daha merkezi olabilir.
    public Guid UserId { get; set; }

    /// <summary>
    /// Kullanıcının adresi kolayca ayırt etmesi için verdiği başlık (Örn: "Ev Adresim", "İş Adresim").
    /// </summary>
    public string AddressTitle { get; set; } = string.Empty;

    /// <summary>
    /// Ülke bilgisi.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Şehir (İl) bilgisi.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// İlçe bilgisi.
    /// </summary>
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// Sokak, cadde, mahalle gibi detayları içeren alan.
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Adresin tam ve açık metin hali.
    /// </summary>
    public string FullAddress { get; set; } = string.Empty;

    /// <summary>
    /// Posta kodu (İsteğe bağlı).
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Bu adresin fatura adresi olarak kullanılıp kullanılamayacağını belirtir.
    /// </summary>
    public bool IsBillingAddress { get; set; } = false;

    /// <summary>
    /// Bu adresin teslimat adresi olarak kullanılıp kullanılamayacağını belirtir.
    /// </summary>
    public bool IsShippingAddress { get; set; } = false;

    /// <summary>
    /// Adresin ait olduğu User entity'si için navigation property.
    /// </summary>
    public virtual User User { get; set; } = null!;

    // Base Entity'den gelenler: Id, CreatedTime, UpdateTime

}