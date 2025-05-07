namespace ETicaret.Domain.Enums;

public enum OrderStatus
{
    Pending,      // Beklemede
    Processing,   // İşleniyor
    Shipped,      // Kargolandı
    Delivered,    // Teslim Edildi
    Cancelled,    // İptal Edildi
    Returned      // İade Edildi
}