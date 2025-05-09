using Core.Persistence.Entities;

namespace ETicaret.Domain.Entities;

/// <summary>
/// Kullanıcının teslimat veya fatura adres bilgilerini temsil eder.
/// </summary>
public class Address : Entity<int>
{
    public string? UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public string AddressTitle { get; set; } // Örn: "Ev Adresim", "İş Adresim"
    public string Country { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string? ZipCode { get; set; } // Posta kodu, nullable olabilir
    public string AddressLine1 { get; set; } // Cadde, Sokak, No
    public string? AddressLine2 { get; set; } // Apartman Adı, Daire No vb., nullable olabilir
    public bool IsDefaultShipping { get; set; } // Varsayılan kargo adresi mi?
    public bool IsDefaultBilling { get; set; } // Varsayılan fatura adresi mi?

    // Base Entity'den gelenler: Id, CreatedTime, UpdateTime

}