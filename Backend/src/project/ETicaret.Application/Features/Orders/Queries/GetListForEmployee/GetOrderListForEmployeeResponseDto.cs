// using Core.Application.Pipelines.Authorization; // Gerekirse yetkilendirme için

namespace ETicaret.Application.Features.Orders.Queries.GetListForEmployee;

public class GetOrderListForEmployeeResponseDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }

    // Müşteri bilgileri (AutoInclude ile User geliyorsa)
    public string? CustomerId { get; set; } // UserId
    public string? CustomerFirstName { get; set; }
    public string? CustomerLastName { get; set; }
    public string? CustomerEmail { get; set; }

    public string? ShippingCity { get; set; }
    public string? ShippingDistrict { get; set; }

    // İsteğe bağlı: Sipariş kalem sayısı
    // public int OrderItemCount { get; set; }
}
