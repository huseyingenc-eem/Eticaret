using Core.Persistence.Entities;

namespace ETicaret.Domain.Entities;

public class Product : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;

    public int? SupplierId { get; set; }
    public virtual Supplier? Supplier { get; set; } = null!;
    public bool IsActive { get; set; } = true;


    public virtual ICollection<ProductVariant> Variants { get; set; } = new HashSet<ProductVariant>();
    public virtual ICollection<ProductImage> Images { get; set; } = new HashSet<ProductImage>();
    public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    public virtual ICollection<DiscountProduct> DiscountProducts { get; set; } = new HashSet<DiscountProduct>();
}