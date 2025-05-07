namespace ETicaret.Application.Features.Products.Queries.GetAllByCategoryId;

public class GetAllByCategoryIdProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}