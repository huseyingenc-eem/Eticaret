using Core.Persistence.Entities;
using ETicaret.Domain.Enums;

namespace ETicaret.Domain.Entities;

public class Payment : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public string? UserId { get; set; }
    public string PaymentMethod { get; set; } // Örn: "CreditCard", "PayPal"
    public string? PaymentProviderTransactionId { get; set; } // Ödeme sağlayıcısından gelen işlem ID'si
    public decimal Amount { get; set; }
    public PaymentStatus PaymentStatus { get; set; } // Örn: "Pending", "Completed", "Failed", "Refunded"
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual Order Order { get; set; }
    public virtual User? User { get; set; }
}
