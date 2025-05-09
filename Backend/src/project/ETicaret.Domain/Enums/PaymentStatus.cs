namespace ETicaret.Domain.Enums;

public enum PaymentStatus
{
    Pending = 1,        // Beklemede
    Completed = 2,      // Tamamlandı
    Failed = 3,         // Başarısız
    Refunded = 4,       // İade Edildi
    Cancelled = 5,      // İptal Edildi
    PartiallyRefunded = 6 // Kısmen İade Edildi
}