using Core.Domain.Entities;

namespace ETicaret.Domain.Entities;

/// <summary>
/// Kullanıcının teslimat veya fatura adres bilgilerini temsil eder.
/// </summary>
public class Address : Entity<Guid>
{
    public string? UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public string AddressTitle { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string? ZipCode { get; set; }
    public string AddressLine { get; set; }

    public string? PhoneNumber { get; set; }
    public bool IsDefaultShipping { get; set; }
    public bool IsDefaultBilling { get; set; }

}