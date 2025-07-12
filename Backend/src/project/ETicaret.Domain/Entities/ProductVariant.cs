using Core.Domain.Entities;

namespace ETicaret.Domain.Entities;

public class ProductVariant : Entity<Guid>
{
    public Guid ProductId { get; set; }
    public virtual Product? Product { get; set; }


    public string Sku { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int UnitsInStock { get; set; }
    public string? VariantImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public string? AttributeDescription { get; set; }
    

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
    public virtual ICollection<CardItem> CartItems { get; set; } = new HashSet<CardItem>();
    public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new HashSet<WishlistItem>();
}
