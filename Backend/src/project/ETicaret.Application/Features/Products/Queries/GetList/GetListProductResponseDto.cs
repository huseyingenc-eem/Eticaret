namespace ETicaret.Application.Features.Products.Queries.GetList;

public class GetListProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; } // double -> decimal
    public int Stock { get; set; }
    public string? ImageUrl { get; set; } // Yeni eklendi
    public bool IsActive { get; set; } // Yeni eklendi
}