namespace ETicaret.Application.Features.Categories.Commands.Update;

// Yeni Response DTO
public class UpdateCategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdateTime { get; set; } // Güncellenme zamanı
    public string Message { get; set; } = string.Empty;
}