using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Queries.GetCategoryTree;

public class GetCategoryTreeResponseDto : IMapFrom<Category>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? ParentId { get; set; }
    public bool IsActive { get; set; }
    public List<GetCategoryTreeResponseDto> Children { get; set; } = new();
}