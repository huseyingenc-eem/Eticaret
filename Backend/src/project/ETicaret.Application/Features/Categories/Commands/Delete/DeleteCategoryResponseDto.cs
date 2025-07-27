namespace ETicaret.Application.Features.Categories.Commands.Delete;

public class DeleteCategoryResponseDto
{

    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool IsSuccess { get; set; }
}
