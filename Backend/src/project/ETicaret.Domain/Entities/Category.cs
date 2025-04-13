using Core.Persistence.Entities;

namespace ETicaret.Domain.Entities;

public class Category : Entity<int>
{
    public string? Name { get; set; }

    public int? ParentId { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category>? Children { get; set; }

    public ICollection<Product>? Products { get; set; }
}
