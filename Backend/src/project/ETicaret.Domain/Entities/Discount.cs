using Core.Domain.Entities;

namespace ETicaret.Domain.Entities;

// --- Promosyon Varlıkları ---

public class Discount : Entity<Guid>
{
    public string Name { get; set; } // "YAZ İNDİRİMİ", "SİYAH CUMA"
    public string? DiscountCode { get; set; } // Eğer kupon ise (örn: "INDIRIM25") - Benzersiz olmalı
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; } // Enum: Percentage, FixedAmount
    public decimal DiscountValue { get; set; } // Tipine göre yüzde veya miktar
    public decimal? MinimumPurchaseAmount { get; set; } // Uygulanması için minimum sepet tutarı
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; } // Bitiş tarihi null ise süresiz
    public int? MaxUses { get; set; } // Toplam maksimum kullanım sayısı
    public int? MaxUsesPerUser { get; set; } // Kullanıcı başına maksimum kullanım
    public bool IsActive { get; set; } = true;
    public bool IsGlobal { get; set; } = false; // True ise tüm sepete uygulanır (MinPurchase dikkate alınarak)

    // Navigation Properties
    public virtual ICollection<DiscountUsage> Usages { get; set; } = new HashSet<DiscountUsage>();
    public virtual ICollection<DiscountProduct> ApplicableProducts { get; set; } = new HashSet<DiscountProduct>();
    public virtual ICollection<DiscountCategory> ApplicableCategories { get; set; } = new HashSet<DiscountCategory>();
}

public enum DiscountType
{
    Percentage,  
    FixedAmount
}