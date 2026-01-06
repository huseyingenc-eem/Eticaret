using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.ProductVariants.Commands.Delete;

public sealed record DeleteProductVariantResponseDto : IMapFrom<ProductVariant>
{
    public required Guid Id { get; init; }
    public required string Sku { get; init; }
    public required decimal Price { get; init; }
    public required bool IsActive { get; init; }
    public required string Message { get; set; }
}