using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Suppliers.Commands.Create;

public sealed record CreateSupplierResponseDto : IMapFrom<Supplier>
{
    public required Guid Id { get; init; }
    public required string CompanyName { get; init; }
    public string? ContactPerson { get; init; }
    public string? ContactEmail { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Address { get; init; }
    public required bool IsActive { get; init; }
    public required DateTime CreatedTime { get; init; }
    public required string Message { get; set; }
}