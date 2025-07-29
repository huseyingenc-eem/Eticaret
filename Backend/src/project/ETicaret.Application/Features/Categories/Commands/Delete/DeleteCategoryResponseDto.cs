using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Commands.Delete;

public class DeleteCategoryResponseDto : IMapFrom<Category>
{

    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool IsSuccess { get; set; }
}
