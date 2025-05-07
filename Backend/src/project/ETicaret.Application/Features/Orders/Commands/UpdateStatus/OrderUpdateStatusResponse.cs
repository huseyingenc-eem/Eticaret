namespace ETicaret.Application.Features.Orders.Commands.UpdateStatus;

// 4. Response Sınıfı (Aynı dosyada, dışarıda)
public class OrderUpdateStatusResponse
{
    public int OrderId { get; set; }
    public string CurrentStatus { get; set; } = string.Empty; // Güncel durumu string olarak döndürelim
    public DateTime? UpdateTime { get; set; } // Güncellenme zamanını döndürebiliriz
}
