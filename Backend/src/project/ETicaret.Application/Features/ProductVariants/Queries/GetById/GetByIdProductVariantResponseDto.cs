using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.ProductVariants.Queries.GetById;

public sealed record GetByIdProductVariantResponseDto : IMapFrom<ProductVariant>
{
    public required Guid Id { get; init; }
    public required Guid ProductId { get; init; }
    public required string ProductName { get; init; }
    public required string Sku { get; init; }
    public required decimal Price { get; init; }
    public decimal? CompareAtPrice { get; init; }
    public required int UnitsInStock { get; init; }
    public string? VariantImageUrl { get; init; }
    public required bool IsActive { get; init; }
    public string? AttributeDescription { get; init; }
    public required DateTime CreatedTime { get; init; }
    public DateTime? UpdateTime { get; init; }

    public required ProductInfoDto Product { get; init; }
}

public sealed record ProductInfoDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required int CategoryId { get; init; }
    public required string CategoryName { get; init; }
    public required bool IsActive { get; init; }
}