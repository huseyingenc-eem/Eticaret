namespace ETicaret.Application.Features.Products.Queries.GetListPriceRange;

public class GetListProductPriceRangeResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
    public string CategoryName { get; set; }
}