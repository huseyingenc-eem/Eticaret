using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Suppliers.Commands.Delete;

public sealed record DeleteSupplierResponseDto : IMapFrom<Supplier>
{
    public required Guid Id { get; init; }
    public required string CompanyName { get; init; }
    public required string Message { get; set; }
}