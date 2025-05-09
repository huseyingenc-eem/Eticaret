using Core.Persistence.Entities;

namespace ETicaret.Domain.Entities;

public class Shipment : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public string? TrackingNumber { get; set; }
    public string? ShippingCarrier { get; set; } // Kargo Firması
    public DateTime? ShippingDate { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public string ShipmentStatus { get; set; }


    public virtual Order Order { get; set; }
    public virtual ICollection<ShipmentItem> ShipmentItems { get; set; } = new HashSet<ShipmentItem>();
}
