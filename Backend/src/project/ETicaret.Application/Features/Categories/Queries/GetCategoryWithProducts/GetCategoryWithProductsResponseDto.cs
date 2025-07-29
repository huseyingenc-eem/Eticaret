using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Queries.GetCategoryWithProducts;

public class GetCategoryWithProductsResponseDto : IMapFrom<Category>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    public List<CategoryProductDto> Products { get; set; } = new();
}

public class CategoryProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}