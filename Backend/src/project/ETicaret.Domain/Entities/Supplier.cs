using Core.Persistence.Entities;
using System.Collections.Generic; // ICollection ve List için

namespace ETicaret.Domain.Entities;

public class Supplier : Entity<int>
{
    // Name (Şirket Adı varsayıldı) zorunlu hale getirildi
    public string Name { get; set; } = string.Empty;

    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    // Base Entity'den gelenler: Id, CreatedTime, UpdateTime

}