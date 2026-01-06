using System;
using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Queries.GetById;

public class GetByIdCategoryResponseDto : IMapFrom<Category>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    
    public string? ParentName { get; set; }
    
    public int ChildrenCount { get; set; }
    public int ProductCount { get; set; }
}
