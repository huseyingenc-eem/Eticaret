namespace ETicaret.Application.Features.Products.Queries.GetDetails;

public class GetDetailsProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }

    public string CategoryName { get; set; }


}
