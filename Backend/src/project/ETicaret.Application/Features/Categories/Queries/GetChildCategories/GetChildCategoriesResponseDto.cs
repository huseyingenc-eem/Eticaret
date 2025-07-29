using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Queries.GetChildCategories;

public class GetChildCategoriesResponseDto : IMapFrom<Category>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; } 
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}