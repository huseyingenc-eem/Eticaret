// using ETicaret.Application.Services.RedisServices;

namespace ETicaret.Application.Features.Suppliers.Commands.Delete;

// Response DTO
public class SupplierDeleteResponseDto
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
}