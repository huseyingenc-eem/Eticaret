using Core.Persistence.Entities;
using ETicaret.Domain.Enums;

namespace ETicaret.Domain.Entities;

public class Order : Entity<int>
{
    public string UserId { get; set; } = string.Empty; 

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }

    public int ShippingAddressId { get; set; }

    public int BillingAddressId { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Address ShippingAddress { get; set; } = null!;

    public virtual Address BillingAddress { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // OrderItem entity'si oluşturulacak

    public Order()
    {
        OrderDate = DateTime.UtcNow; // Oluşturulduğunda varsayılan tarih atanabilir
        Status = OrderStatus.Pending; // Varsayılan durum atanabilir
    }
}