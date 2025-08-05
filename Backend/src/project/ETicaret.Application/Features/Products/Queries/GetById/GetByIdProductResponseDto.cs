using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Products.Queries.GetById;

public class GetByIdProductResponseDto : IMapFrom<Product>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public List<ProductVariantDto> Variants { get; set; } = new();
    public List<ProductImageDto> Images { get; set; } = new();
}

public class ProductVariantDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int UnitsInStock { get; set; }
    public string? VariantImageUrl { get; set; }
    public string? AttributeDescription { get; set; }
    public bool IsActive { get; set; }
}

public class ProductImageDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
}