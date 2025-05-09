using Core.Persistence.Entities;

namespace ETicaret.Domain.Entities;

public class ShipmentItem : Entity<Guid> 
{
    public Guid ShipmentId { get; set; }
    public Guid OrderItemId { get; set; }
    public int QuantityShipped { get; set; }

    // Navigation Properties
    public virtual Shipment Shipment { get; set; }
    public virtual OrderItem OrderItem { get; set; }
}
