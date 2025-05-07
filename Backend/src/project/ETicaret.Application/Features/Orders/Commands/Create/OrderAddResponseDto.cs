namespace ETicaret.Application.Features.Orders.Commands.Create;

public class OrderAddResponseDto
{
    public int OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
}
