namespace ETicaret.Application.Features.Products.Commands.Update;

// Yeni Response DTO (Örnek)
public class ProductUpdateResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    // İstenirse diğer alanlar da eklenebilir
}