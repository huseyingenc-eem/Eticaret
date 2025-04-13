namespace ETicaret.Application.Features.Products.Queries.GetAllByCategoryId;

public class GetAllByCategoryIdProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }

    public string CategoryName { get; set; }
}
