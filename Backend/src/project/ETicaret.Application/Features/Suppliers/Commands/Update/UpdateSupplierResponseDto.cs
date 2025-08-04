using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Suppliers.Commands.Update;

/// <summary>
/// Tedarikçi güncelleme işlemi sonrası dönen immutable response DTO.
/// Record pattern kullanılarak performans optimize edilmiştir.
/// </summary>
public sealed record UpdateSupplierResponseDto : IMapFrom<Supplier>
{
    public required Guid Id { get; init; }
    public required string CompanyName { get; init; }
    public string? ContactPerson { get; init; }
    public string? ContactEmail { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Address { get; init; }
    public required bool IsActive { get; init; }
    public required DateTime UpdateTime { get; init; }
    public string Message { get; set; } = string.Empty;
}