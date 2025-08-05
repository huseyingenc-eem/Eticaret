using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Discounts.Commands.Delete;

public sealed record DeleteDiscountResponseDto : IMapFrom<Discount>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? DiscountCode { get; init; }
    public required string Message { get; set; }
}