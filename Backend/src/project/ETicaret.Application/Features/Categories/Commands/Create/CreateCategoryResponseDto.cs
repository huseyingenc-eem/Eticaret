namespace ETicaret.Application.Features.Categories.Commands.Create;

public class CreateCategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public string Message { get; set; } = string.Empty;
}