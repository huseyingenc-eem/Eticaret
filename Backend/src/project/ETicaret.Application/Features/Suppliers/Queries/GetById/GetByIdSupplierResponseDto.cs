namespace ETicaret.Application.Features.Suppliers.Queries.GetById;

public class GetByIdSupplierResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    //public List<SupplierProductDto> Products { get; set; } = new();
}

//public class SupplierProductDto
//{
//    public int Id { get; set; }
//    public string Name { get; set; } = string.Empty;
//    public decimal Price { get; set; }
//    public int Stock { get; set; }
//    public string? SKU { get; set; }
//    public string? ImageUrl { get; set; }
//    public bool IsActive { get; set; }
//}