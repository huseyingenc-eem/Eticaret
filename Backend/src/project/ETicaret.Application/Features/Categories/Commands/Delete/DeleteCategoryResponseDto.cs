namespace ETicaret.Application.Features.Categories.Commands.Update;

public class DeleteCategoryResponseDto
{

    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool IsSuccess { get; set; }
}
