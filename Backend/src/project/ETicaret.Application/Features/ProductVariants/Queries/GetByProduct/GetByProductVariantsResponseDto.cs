using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.ProductVariants.Queries.GetByProduct;

public sealed record GetByProductVariantsResponseDto : IMapFrom<ProductVariant>
{
    public required Guid Id { get; init; }
    public required string Sku { get; init; }
    public required decimal Price { get; init; }
    public decimal? CompareAtPrice { get; init; }
    public required int UnitsInStock { get; init; }
    public string? VariantImageUrl { get; init; }
    public required bool IsActive { get; init; }
    public string? AttributeDescription { get; init; }

    public bool IsInStock => UnitsInStock > 0;
    public bool IsOnSale => CompareAtPrice.HasValue && CompareAtPrice > Price;
    public decimal? SavingsAmount => CompareAtPrice.HasValue ? CompareAtPrice - Price : null;
    public decimal? SavingsPercentage => CompareAtPrice.HasValue ? Math.Round(((CompareAtPrice.Value - Price) / CompareAtPrice.Value) * 100, 2) : null;
}