namespace ETicaret.Application.Features.Products.Commands.Create;

// Yeni Response DTO (Örnek)
public class ProductAddResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    // İstenirse diğer alanlar da eklenebilir
}