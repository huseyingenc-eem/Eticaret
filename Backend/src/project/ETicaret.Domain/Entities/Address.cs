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
    public string? UserId { get; set; }

    public string AddressTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public bool IsBillingAddress { get; set; } = false;
    public bool IsShippingAddress { get; set; } = false;
    public virtual User User { get; set; } = null!;

    // Base Entity'den gelenler: Id, CreatedTime, UpdateTime

}