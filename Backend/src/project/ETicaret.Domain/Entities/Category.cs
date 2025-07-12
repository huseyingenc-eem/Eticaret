using Core.Domain.Entities;

namespace ETicaret.Domain.Entities;

public class Category : Entity<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public int? ParentId { get; set; }
    public virtual Category? Parent { get; set; }
    public virtual ICollection<Category> Children { get; set; } = new HashSet<Category>();
    public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    public virtual ICollection<DiscountCategory> DiscountCategories { get; set; } = new HashSet<DiscountCategory>();
}