namespace ETicaret.Application.Features.Products.Commands.Create;

public class CreateProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}