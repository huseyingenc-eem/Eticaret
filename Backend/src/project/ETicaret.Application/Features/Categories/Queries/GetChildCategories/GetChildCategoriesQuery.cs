using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Queries.GetChildCategories;

public class GetChildCategoriesQuery : IRequest<List<GetChildCategoriesResponseDto>>,
    ICachableRequest,
    IPublicRequest
{
    public int ParentId { get; set; }
    public bool OnlyActive { get; set; } = true;

    #region Cache Settings
    public bool BypassCache { get; set; }
    public string CacheKey => $"child-categories_parent_{ParentId}_active_{OnlyActive}";
    public string? CacheGroupKey => "Categories";
    public TimeSpan? SlidingExpiration => TimeSpan.FromHours(1);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(4);
    #endregion

    public class GetChildCategoriesQueryHandler : IRequestHandler<GetChildCategoriesQuery, List<GetChildCategoriesResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Category, int> _categoryRepository;

        public GetChildCategoriesQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _categoryRepository = unitOfWork.GetRepository<Category, int>();
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

            if (request.OnlyActive)
            {
                categories = categories.Where(c => c.IsActive).ToList();
            }

            if (!categories.Any())
            {
                throw new NotFoundException(
                    message: $"No child categories found for parent category with ID {request.ParentId}.",
                    userFriendlyMessage: "Bu kategorinin alt kategorisi bulunmuyor.",
                    errorCode: "NO_CHILD_CATEGORIES_FOUND"
                );
            }

            return _mapper.Map<List<GetChildCategoriesResponseDto>>(categories);
        }
    }
}