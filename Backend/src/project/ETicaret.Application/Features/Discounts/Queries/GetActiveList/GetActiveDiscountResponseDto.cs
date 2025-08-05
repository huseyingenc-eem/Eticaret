using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Discounts.Queries.GetActiveList;

public class GetActiveDiscountResponseDto : IMapFrom<Discount>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DiscountCode { get; set; }
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinimumPurchaseAmount { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsGlobal { get; set; }
}