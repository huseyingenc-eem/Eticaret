using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Queries.GetParentCategories;

public class GetParentCategoriesResponseDto : IMapFrom<Category>
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public string? Description { get; set; }
    public bool IsActive { get; set; }

}
