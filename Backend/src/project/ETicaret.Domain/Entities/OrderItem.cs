using Core.Domain.Entities;

namespace ETicaret.Domain.Entities;

public class OrderItem : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } 
    public decimal TotalPrice { get; set; }
    public virtual Order Order { get; set; }
    public virtual ProductVariant ProductVariant { get; set; }
    public virtual ICollection<ShipmentItem> ShipmentItems { get; set; } = new HashSet<ShipmentItem>();
}
