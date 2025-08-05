using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Discounts.Commands.Create;

public sealed record CreateDiscountResponseDto : IMapFrom<Discount>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? DiscountCode { get; init; }
    public string? Description { get; init; }
    public required DiscountType DiscountType { get; init; }
    public required decimal DiscountValue { get; init; }
    public decimal? MinimumPurchaseAmount { get; init; }
    public required DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int? MaxUses { get; init; }
    public int? MaxUsesPerUser { get; init; }
    public required bool IsActive { get; init; }
    public required bool IsGlobal { get; init; }
    public required DateTime CreatedTime { get; init; }
    public required string Message { get; set; }
}