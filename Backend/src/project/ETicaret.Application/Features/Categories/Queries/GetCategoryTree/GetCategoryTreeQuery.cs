using AutoMapper;
using Core.Application.Abstractions.Repositories;
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
public class GetCategoryTreeQuery : IRequest<List<GetCategoryTreeResponseDto>>, ICachableRequest
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

    /// <summary>
    /// Kategori ağacı getirme sorgusunu işler.
    /// Performans için O(n) karmaşıklığında tek geçişli algoritma kullanır.
    /// </summary>
    /// <param name="request">Kategori ağacı getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Hiyerarşik kategori ağacının listesi.</returns>
    public async Task<List<GetCategoryTreeResponseDto>> Handle(GetCategoryTreeQuery request, CancellationToken cancellationToken)
    {
        // 1. Tüm kategorileri tek sorgu ile veritabanından getir
        List<Category> allCategories = await GetAllCategoriesAsync(cancellationToken);

        // 2. Sonuç kontrolü yap
        ValidateResults(allCategories);

        // 3. Entity'leri DTO'ya dönüştür
        List<GetCategoryTreeResponseDto> categoryDtos = MapToResponseDtos(allCategories);

        // 4. Hiyerarşik ağaç yapısını oluştur ve döndür
        return BuildCategoryTree(categoryDtos);
    }

    #endregion

    #region Yardımcı Metotlar (Helper Methods)

    /// <summary>
    /// Tüm kategorileri veritabanından getirir.
    /// </summary>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Kategori entity'lerinin listesi.</returns>
    private async Task<List<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Yeni specification sistemini kullanarak tüm kategorileri getir
            var allCategoriesSpec = new CategorySpecifications.Active();
            return await _categoryRepository.GetListAsync(allCategoriesSpec, cancellationToken);
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

    /// <summary>
    /// Sorgu sonuçlarını doğrular.
    /// </summary>
    /// <param name="categories">Doğrulanacak kategori listesi.</param>
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

    /// <summary>
    /// Kategori entity listesini yanıt DTO listesine dönüştürür.
    /// </summary>
    /// <param name="categories">Dönüştürülecek kategori entity'leri.</param>
    /// <returns>Yanıt DTO'larının listesi.</returns>
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
            // 1. Performans için kategori ID'lerini hızlı erişim sözlüğü haline getir
            var categoryMap = categoryDtos.ToDictionary(c => c.Id);

            // 2. Kök kategorileri tutacak sonuç listesi
            var rootCategories = new List<GetCategoryTreeResponseDto>();

            // 3. TEK GEÇİŞTE hiyerarşi oluştur (O(n) karmaşıklık)
            foreach (var category in categoryDtos)
            {
                if (category.ParentId == null)
                {
                    // Ana kategori (kök düzey)
                    rootCategories.Add(category);
                }
                else
                {
                    // Alt kategori - ebeveynini bul ve ekle
                    if (categoryMap.TryGetValue(category.ParentId.Value, out var parentCategory))
                    {
                        parentCategory.Children ??= new List<GetCategoryTreeResponseDto>();
                        parentCategory.Children.Add(category);
                    }
                    // Not: Ebeveyn bulunamazsa (orphan kategori), o kategori ağaçta görünmeyecek
                    // Bu, veri bütünlüğü sorunu olabilir ancak uygulama çökmez
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