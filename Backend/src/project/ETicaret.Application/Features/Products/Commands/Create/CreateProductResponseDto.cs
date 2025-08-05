using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Products.Commands.Create;

public class CreateProductResponseDto : IMapFrom<Product>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public Guid? SupplierId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public string Message { get; set; } = string.Empty;
}