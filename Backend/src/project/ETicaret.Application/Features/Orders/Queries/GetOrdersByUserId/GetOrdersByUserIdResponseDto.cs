namespace ETicaret.Application.Features.Orders.Queries.GetListByUserId;

public class GetOrdersByUserIdResponseDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}
