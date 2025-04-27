using Core.Persistence.Entities;

namespace ETicaret.Domain.Entities;

public class Category : Entity<int>
{
    // Name alanı zorunlu hale getirildi ve başlangıç değeri atandı
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; } // İsteğe bağlı açıklama
    public bool IsActive { get; set; } = true; // Varsayılan olarak aktif

    // Mevcut Hiyerarşi Alanları (Parent/Children)
    public int? ParentId { get; set; }
    public virtual Category? Parent { get; set; } // Lazy loading için virtual
    public virtual ICollection<Category> Children { get; set; } = new List<Category>(); // Başlangıç değeri atandı ve virtual yapıldı

    // Product ilişkisi için koleksiyon (non-nullable, virtual ve initialize edildi)
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    // Base Entity'den gelenler: Id, CreatedTime, UpdateTime (Eğer base entity'de varsa)

}