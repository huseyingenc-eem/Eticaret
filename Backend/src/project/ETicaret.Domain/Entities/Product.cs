using Core.Persistence.Entities;

namespace ETicaret.Domain.Entities;

public class Product : Entity<int>
{
    public string? Name { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }

    public int CategoryID { get; set; }
    public Category? Category { get; set; }

    public int SupplierID { get; set; }
    public Supplier? Supplier { get; set; }
}

 