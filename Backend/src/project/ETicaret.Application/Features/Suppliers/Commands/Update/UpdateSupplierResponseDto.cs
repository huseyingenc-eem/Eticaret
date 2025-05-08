namespace ETicaret.Application.Features.Suppliers.Commands.Update;

// Response DTO
public class UpdateSupplierResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; } // Güncellenen diğer bilgiler de eklenebilir
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public string Message { get; set; } = string.Empty;
}