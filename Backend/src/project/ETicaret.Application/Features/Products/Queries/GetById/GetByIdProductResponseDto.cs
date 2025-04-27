namespace ETicaret.Application.Features.Products.Queries.GetById;

public class GetByIdProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; } // double -> decimal
    public int Stock { get; set; }
    public int CategoryID { get; set; }
    public int SupplierID { get; set; } // Yeni eklendi
    public string? Description { get; set; } // Yeni eklendi
    public string? SKU { get; set; } // Yeni eklendi
    public string? ImageUrl { get; set; } // Yeni eklendi
    public bool IsActive { get; set; } // Yeni eklendi
    // İstenirse CategoryName ve SupplierName de buraya eklenebilir
}