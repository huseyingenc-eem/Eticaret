namespace ETicaret.Application.Features.Suppliers.Commands.Create;

// Response DTO (Aynı dosyada veya ayrı bir yerde olabilir)
public class CreateSupplierResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public string Message { get; set; } = string.Empty;
}