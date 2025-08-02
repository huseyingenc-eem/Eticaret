using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Queries.GetCategoryTree;

#region Sorgu Sınıfı (Query Class)

/// <summary>
/// Tüm kategorileri hiyerarşik bir ağaç yapısı olarak getiren sorgu.
/// ICachableRequest: Bu sorgunun sonucunun önbelleğe alınmasını sağlar.
/// Kategori ağacı sık değişmediği için önbellekleme performans açısından oldukça yararlıdır.
/// </summary>
public class GetCategoryTreeQuery : IRequest<List<GetCategoryTreeResponseDto>>,
    ICachableRequest,
    IPublicRequest
{
    #region Önbellek Ayarları (Cache Settings)
    public bool BypassCache { get; set; }
    public string CacheKey => "category-tree";
    public string? CacheGroupKey => "Categories";
    public TimeSpan? SlidingExpiration => TimeSpan.FromHours(2);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(6);

    #endregion
}

#endregion

#region Sorgu İşleyici (Query Handler)
public class GetCategoryTreeQueryHandler : IRequestHandler<GetCategoryTreeQuery, List<GetCategoryTreeResponseDto>>
{
    #region Alan Tanımlamaları (Field Declarations)

    private readonly IRepository<Category, int> _categoryRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Yapıcı Metot (Constructor)
    public GetCategoryTreeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _categoryRepository = unitOfWork.GetRepository<Category, int>();
        _mapper = mapper;
    }

    #endregion

    #region İşleme Metotları (Handler Methods)
    public async Task<List<GetCategoryTreeResponseDto>> Handle(GetCategoryTreeQuery request, CancellationToken cancellationToken)
    {
        List<Category> allCategories = await GetAllCategoriesAsync(cancellationToken);

        ValidateResults(allCategories);

        List<GetCategoryTreeResponseDto> categoryDtos = MapToResponseDtos(allCategories);

        return BuildCategoryTree(categoryDtos);
    }

    #endregion

    #region Yardımcı Metotlar (Helper Methods)

    private async Task<List<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var treeDataSpec = new CategorySpecifications.TreeData();
            return await _categoryRepository.GetListAsync(treeDataSpec, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new BusinessException(
                message: "Failed to retrieve categories from database.",
                userFriendlyMessage: "Kategoriler veritabanından alınırken bir hata oluştu. Lütfen tekrar deneyiniz.",
                errorCode: "CATEGORIES_RETRIEVAL_FAILED",
                additionalData: new { Exception = ex.Message }
            );
        }
    }
    private static void ValidateResults(List<Category> categories)
    {
        if (!categories.Any())
        {
            throw new NotFoundException(
                message: "No categories found in the system.",
                userFriendlyMessage: "Sistemde henüz kategori bulunmuyor.",
                errorCode: "NO_CATEGORIES_FOUND"
            );
        }
    }

    private List<GetCategoryTreeResponseDto> MapToResponseDtos(List<Category> categories)
    {
        try
        {
            return _mapper.Map<List<GetCategoryTreeResponseDto>>(categories);
        }
        catch (Exception ex)
        {
            throw new BusinessException(
                message: "Failed to map categories to response DTOs.",
                userFriendlyMessage: "Kategori bilgileri işlenirken bir hata oluştu.",
                errorCode: "CATEGORY_MAPPING_FAILED",
                additionalData: new { CategoryCount = categories.Count, Exception = ex.Message }
            );
        }
    }

    /// <summary>
    /// Kategori DTO'larından hiyerarşik ağaç yapısını oluşturur.
    /// PERFORMANS OPTİMİZASYONU: O(n) karmaşıklığında tek geçişli algoritma kullanır.
    /// </summary>
    /// <param name="categoryDtos">Ağaç yapısına dönüştürülecek kategori DTO'ları.</param>
    /// <returns>Kök kategorileri içeren hiyerarşik liste.</returns>
    private static List<GetCategoryTreeResponseDto> BuildCategoryTree(List<GetCategoryTreeResponseDto> categoryDtos)
    {
        try
        {
            var uniqueCategories = categoryDtos
                .GroupBy(c => c.Id)
                .Select(g =>
                {
                    var category = g.First();
                    category.Children = new List<GetCategoryTreeResponseDto>();
                    return category;
                })
                .ToList();

            var categoryMap = uniqueCategories.ToDictionary(c => c.Id);
            var rootCategories = new List<GetCategoryTreeResponseDto>();

            foreach (var category in uniqueCategories)
            {
                if (category.ParentId == null)
                {
                    rootCategories.Add(category);
                }
                else
                {
                    if (categoryMap.TryGetValue(category.ParentId.Value, out var parentCategory))
                    {
                        if (!parentCategory.Children.Any(c => c.Id == category.Id))
                        {
                            parentCategory.Children.Add(category);
                        }
                    }
                }
            }
            return rootCategories;
        }
        catch (Exception ex)
        {
            throw new BusinessException(
                message: "Failed to build category tree structure.",
                userFriendlyMessage: "Kategori ağacı oluşturulurken bir hata oluştu.",
                errorCode: "CATEGORY_TREE_BUILD_FAILED",
                additionalData: new { CategoryCount = categoryDtos.Count, Exception = ex.Message }
            );
        }
    }

    #endregion
}

#endregion