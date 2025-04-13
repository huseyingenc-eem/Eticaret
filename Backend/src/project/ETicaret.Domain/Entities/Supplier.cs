using Core.Persistence.Entities;

namespace ETicaret.Domain.Entities;

public class Supplier : Entity<int>
{
    public string? Name { get; set; }
    public string? Surname { get; set; }

    public ICollection<Product>? Products { get; set; }
}
