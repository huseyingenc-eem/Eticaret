using Core.Persistence.Entities;
using System.ComponentModel.DataAnnotations; // Gerekli olabilir

namespace ETicaret.Domain.Entities;

public class Product : Entity<int>
{
    // Mevcut Alanlar (Zorunlu Hale Getirildi)
    [Required] // Fluent Validation veya Configuration'da da tanımlanabilir
    [MaxLength(200)] // Örnek uzunluk kısıtlaması
    public string Name { get; set; } = string.Empty; // Non-nullable string için başlangıç değeri

    [Required] // Fiyat zorunlu
    public decimal Price { get; set; }

    [Required] // Stok zorunlu
    public int Stock { get; set; }

    [Required] // Kategori ID zorunlu
    public int CategoryID { get; set; }
    public virtual Category Category { get; set; } = null!; // Navigation property (non-nullable)

    [Required] // Tedarikçi ID zorunlu
    public int SupplierID { get; set; }
    public virtual Supplier Supplier { get; set; } = null!; // Navigation property (non-nullable)

    // Yeni Eklenen Alanlar
    public string? Description { get; set; } // Açıklama (isteğe bağlı)

    [MaxLength(100)] // Örnek uzunluk kısıtlaması
    public string? SKU { get; set; } // Stok Takip Birimi (isteğe bağlı veya zorunluysa Required ekleyin)

    public string? ImageUrl { get; set; } // Resim URL'si (isteğe bağlı)

    public bool IsActive { get; set; } = true; // Varsayılan olarak aktif

    // Base Entity'den gelenler: Id, CreatedTime, UpdateTime

}