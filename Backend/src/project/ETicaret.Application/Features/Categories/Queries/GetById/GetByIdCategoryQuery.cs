using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Specifications;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Queries.GetById;

#region Get By Id Category Query

public class GetByIdCategoryQuery : IRequest<GetByIdCategoryResponseDto>,
    ICachableRequest,
    IPublicRequest
{
    #region Properties
    public int Id { get; set; }
    #endregion

    #region Cache Settings
    public bool BypassCache { get; set; }
    public string CacheKey => $"category-detail_{Id}";
    public string? CacheGroupKey => "Categories";
    public TimeSpan? SlidingExpiration => TimeSpan.FromHours(1);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(4);
    #endregion
}

#endregion

#region Get By Id Category Query Handler

public class GetByIdCategoryQueryHandler : IRequestHandler<GetByIdCategoryQuery, GetByIdCategoryResponseDto>
{
    #region Fields
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    #endregion

    #region Constructor
    public GetByIdCategoryQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _mapper = mapper;
        _categoryRepository = categoryRepository;
    }
    #endregion

    #region Handler Implementation
    public async Task<GetByIdCategoryResponseDto> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
    {
        // ID validation
        if (request.Id <= 0)
        {
            throw new BusinessException(
                message: "Category ID must be greater than 0.",
                userFriendlyMessage: "Geçersiz kategori ID'si.",
                errorCode: "INVALID_CATEGORY_ID"
            );
        }

        // Include parent bilgisi ve count bilgileri için detaylı specification
        var spec = new CategorySpecifications.ByIdWithDetails(request.Id);
        Category? category = await _categoryRepository.GetAsync(spec, cancellationToken);

        if (category == null)
        {
            throw new NotFoundException(
                message: $"Category with ID {request.Id} not found.",
                userFriendlyMessage: "Belirtilen kategori bulunamadı.",
                errorCode: "CATEGORY_NOT_FOUND"
            );
        }

        // DTO'ya mapping
        var response = _mapper.Map<GetByIdCategoryResponseDto>(category);

        // Manual mapping for calculated fields
        response.ParentName = category.Parent?.Name;
        response.ChildrenCount = category.Children?.Count ?? 0;
        response.ProductCount = category.Products?.Count ?? 0;

        return response;
    }
    #endregion
}

#endregion