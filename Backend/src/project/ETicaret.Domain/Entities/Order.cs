using Core.Domain.Entities;
using ETicaret.Domain.Enums;

namespace ETicaret.Domain.Entities;

public class Order : Entity<Guid>
{
    public string UserId { get; set; } = string.Empty;
    public string OrderNumber { get; set; } // Benzersiz, okunabilir sipariş no (örn: 20250508-12345)
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }

    public decimal Subtotal { get; set; } // İndirim ve vergiler öncesi ara toplam
    public decimal DiscountTotal { get; set; } = 0; // Uygulanan toplam indirim
    public decimal ShippingFee { get; set; } = 0; // Kargo ücreti
    public decimal TaxTotal { get; set; } = 0; // Toplam vergi
    public decimal GrandTotal { get; set; } // Ödenecek Genel Toplam
    public string? CustomerNotes { get; set; }

    public Guid ShippingAddressId { get; set; }
    public Guid BillingAddressId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Address ShippingAddress { get; set; } = null!;
    public virtual Address BillingAddress { get; set; } = null!;


    public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    public virtual ICollection<Shipment> Shipments { get; set; } = new HashSet<Shipment>();
    public virtual ICollection<DiscountUsage> AppliedDiscounts { get; set; } = new HashSet<DiscountUsage>();

    public Order()
    {
        OrderDate = DateTime.UtcNow; // Oluşturulduğunda varsayılan tarih atanabilir
        Status = OrderStatus.Pending; // Varsayılan durum atanabilir
    }
}