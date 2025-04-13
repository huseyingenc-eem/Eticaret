namespace ETicaret.Application.Features.Categories.Queries.GetCategoryTree;

public class GetCategoryTreeResponseDto
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public int? ParentId { get; set; }
    public List<GetCategoryTreeResponseDto> Children { get; set; } = new();
}
