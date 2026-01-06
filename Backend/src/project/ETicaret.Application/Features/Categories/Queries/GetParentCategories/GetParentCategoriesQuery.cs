using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using ETicaret.Application.Features.Categories.Specifications;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Queries.GetParentCategories;

#region Sorgu Sınıfı (Query Class)

public class GetParentCategoriesQuery : IRequest<List<GetParentCategoriesResponseDto>>,
    ICachableRequest,
    IPublicRequest
{
    #region Önbellek Ayarları (Cache Settings)
    public bool BypassCache { get; set; }

    public string CacheKey => "parent-categories";
    public string? CacheGroupKey => "Categories";

    public TimeSpan? SlidingExpiration => TimeSpan.FromHours(2);

    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(6);

    #endregion
}

#endregion

#region Sorgu İşleyici (Query Handler)

public class GetParentCategoriesQueryHandler : IRequestHandler<GetParentCategoriesQuery, List<GetParentCategoriesResponseDto>>
{
    #region Alan Tanımlamaları (Field Declarations)

    private readonly IMapper _mapper;
    private readonly ICategoryRepository _categoryRepository;

    #endregion

    #region Yapıcı Metot (Constructor)
    public GetParentCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _mapper = mapper;
        _categoryRepository = categoryRepository;
    }

    #endregion

    #region İşleme Metotları (Handler Methods)

    public async Task<List<GetParentCategoriesResponseDto>> Handle(GetParentCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await GetParentCategoriesAsync(cancellationToken);

        return _mapper.Map<List<GetParentCategoriesResponseDto>>(categories);
    }

    #endregion

    #region Yardımcı Metotlar (Helper Methods)

    private async Task<List<Category>> GetParentCategoriesAsync(CancellationToken cancellationToken)
    {
        var spec = new CategorySpecifications.Parents();
        return await _categoryRepository.GetListAsync(spec, cancellationToken);
    }


    #endregion
}

#endregion