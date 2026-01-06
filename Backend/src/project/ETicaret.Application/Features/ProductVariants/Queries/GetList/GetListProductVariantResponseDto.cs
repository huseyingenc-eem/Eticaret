using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.ProductVariants.Queries.GetList;

public sealed record GetListProductVariantResponseDto : IMapFrom<ProductVariant>
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
    public string StockStatus => UnitsInStock switch
    {
        0 => "Stokta Yok",
        <= 5 => "Düşük Stok",
        <= 20 => "Orta Stok",
        _ => "Yüksek Stok"
    };

    public bool IsOnSale => CompareAtPrice.HasValue && CompareAtPrice > Price;
}