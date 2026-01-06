using AutoMapper;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Specifications;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Categories.Queries.GetChildCategories;

public class GetChildCategoriesQuery : IRequest<List<GetChildCategoriesResponseDto>>,
    ICachableRequest,
    IPublicRequest
{
    public int ParentId { get; set; }
    public bool? OnlyActive { get; set; } = null;

    #region Cache Settings
    public bool BypassCache { get; set; }
    public string CacheKey => $"child-categories_parent_{ParentId}_active_{OnlyActive switch { true => "true", false => "false", null => "null" }}";
    public string? CacheGroupKey => "Categories";
    public TimeSpan? SlidingExpiration => TimeSpan.FromHours(1);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(4);
    #endregion

    public class GetChildCategoriesQueryHandler : IRequestHandler<GetChildCategoriesQuery, List<GetChildCategoriesResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public GetChildCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<GetChildCategoriesResponseDto>> Handle(GetChildCategoriesQuery request, CancellationToken cancellationToken)
        {
            if (request.ParentId <= 0)
            {
                throw new BusinessException(
                    message: "Parent category ID must be greater than 0.",
                    userFriendlyMessage: "Geçersiz üst kategori ID'si.",
                    errorCode: "INVALID_PARENT_CATEGORY_ID"
                );
            }

            var parentSpec = new CategorySpecifications.ById(request.ParentId);
            var parentExists = await _categoryRepository.AnyAsync(parentSpec, cancellationToken);

            if (!parentExists)
            {
                throw new NotFoundException(
                    message: $"Parent category with ID {request.ParentId} not found.",
                    userFriendlyMessage: "Üst kategori bulunamadı.",
                    errorCode: "PARENT_CATEGORY_NOT_FOUND"
                );
            }

            var spec = new CategorySpecifications.ByParentId(request.ParentId);
            var categories = await _categoryRepository.GetListAsync(spec, cancellationToken);

            return _mapper.Map<List<GetChildCategoriesResponseDto>>(categories);
        }
    }
}