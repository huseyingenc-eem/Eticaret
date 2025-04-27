namespace ETicaret.Application.Features.Products.Queries.GetDetails;

public class GetDetailsProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; } // double -> decimal
    public int Stock { get; set; }
    public string CategoryName { get; set; } = string.Empty; // Mevcut
    public int SupplierID { get; set; } 
    public string SupplierName { get; set; } = string.Empty; 
    public string? Description { get; set; }
    public string? SKU { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdateTime { get; set; }
}