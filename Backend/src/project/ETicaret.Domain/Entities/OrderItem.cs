using Core.Persistence.Entities;
using System.ComponentModel.DataAnnotations.Schema; // Price için

namespace ETicaret.Domain.Entities;

public class OrderItem : Entity<int> // Sipariş Kalemi ID'si
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")] 
    public decimal Price { get; set; }

    // --- Navigation Properties ---
    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    // Base Entity'den gelenler: Id, CreatedTime, UpdateTime
}