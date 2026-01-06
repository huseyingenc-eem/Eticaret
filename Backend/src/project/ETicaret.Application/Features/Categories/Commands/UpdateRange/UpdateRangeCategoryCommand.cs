using AutoMapper;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Domain.Entities;
using MediatR;
using Core.Shared.Constants;
using ETicaret.Application.Services.Repositories;

namespace ETicaret.Application.Features.Categories.Commands.UpdateRange;

[DefaultRoles("Admin")]
public class UpdateRangeCategoryCommand : IRequest<UpdateRangeCategoryResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    public int? ParentId { get; set; }
    public List<CategoryUpdateItem> Categories { get; set; } = new();

    #region Cache Settings
    public string? CacheKey => null;
    public string? CacheGroupKey => "Categories";
    #endregion

    public class UpdateRangeCategoryCommandHandler : IRequestHandler<UpdateRangeCategoryCommand, UpdateRangeCategoryResponseDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public UpdateRangeCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<UpdateRangeCategoryResponseDto> Handle(UpdateRangeCategoryCommand request, CancellationToken cancellationToken)
        {
            var categories = new List<Category>();

            foreach (var categoryItem in request.Categories)
            {
                var category = new Category
                {
                    Name = categoryItem.Name,
                    Description = categoryItem.Description,
                    ParentId = request.ParentId,
                    IsActive = categoryItem.IsActive
                };

                categories.Add(category);
            }

            await _categoryRepository.AddRangeAsync(categories, cancellationToken);

            var response = new UpdateRangeCategoryResponseDto
            {
                ParentId = request.ParentId,
                AddedCategoriesCount = categories.Count,
                Categories = _mapper.Map<List<CategoryItemDto>>(categories),
                Message = MessageConstants.Success.EntityAdded($"{categories.Count} kategori")
            };

            return response;
        }
    }
}

/// <summary>
/// Toplu kategori ekleme için kategori öğesi
/// </summary>
public class CategoryUpdateItem
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}