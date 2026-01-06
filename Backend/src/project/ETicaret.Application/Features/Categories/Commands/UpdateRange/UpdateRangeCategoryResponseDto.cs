namespace ETicaret.Application.Features.Categories.Commands.UpdateRange;

public class UpdateRangeCategoryResponseDto
{
    public int? ParentId { get; set; }
    public int AddedCategoriesCount { get; set; }
    public List<CategoryItemDto> Categories { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class CategoryItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
}