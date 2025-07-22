using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Rules;
using ETicaret.Application.Features.Categories.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Queries.GetChildCategories;

#region Sorgu Sınıfı (Query Class)

/// <summary>
/// Belirli bir ebeveyn kategorinin alt kategorilerini getirmek için kullanılan sorgu.
/// ICachableRequest: Bu sorgunun sonucunun önbelleğe alınmasını sağlar.
/// Alt kategoriler sık değişmediği için önbellekleme performans açısından yararlıdır.
/// </summary>
public class GetChildCategoriesQuery : IRequest<List<GetChildCategoriesResponseDto>>, ICachableRequest
{
    #region Özellikler (Properties)

    public int ParentId { get; set; }

    public bool OnlyActive { get; set; } = true;

    #endregion

    #region Önbellek Ayarları (Cache Settings)
    public bool BypassCache { get; set; }
    public string CacheKey => $"child-categories_parent_{ParentId}_active_{OnlyActive}";
    public string? CacheGroupKey => "Categories";
    public TimeSpan? SlidingExpiration => TimeSpan.FromHours(1);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(4);

    #endregion
}

#endregion

#region Sorgu İşleyici (Query Handler)

/// <summary>
/// GetChildCategoriesQuery sorgusunu işleyen handler sınıfı.
/// </summary>
public class GetChildCategoriesQueryHandler : IRequestHandler<GetChildCategoriesQuery, List<GetChildCategoriesResponseDto>>
{
    #region Alan Tanımlamaları (Field Declarations)

    private readonly IMapper _mapper;
    private readonly IRepository<Category, int> _categoryRepository;
    private readonly CategoryBusinessRules _categoryBusinessRules;

    #endregion

    #region Yapıcı Metot (Constructor)
    public GetChildCategoriesQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, CategoryBusinessRules categoryBusinessRules)
    {
        _mapper = mapper;
        _categoryRepository = unitOfWork.GetRepository<Category, int>();
        _categoryBusinessRules = categoryBusinessRules;
    }

    #endregion

    #region İşleme Metotları (Handler Methods)

    /// <summary>
    /// Alt kategorileri getirme sorgusunu işler.
    /// </summary>
    /// <param name="request">Alt kategori getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Alt kategorilerin listesini içeren yanıt DTO'su.</returns>
    public async Task<List<GetChildCategoriesResponseDto>> Handle(GetChildCategoriesQuery request, CancellationToken cancellationToken)
    {
        // 1. İstek parametrelerini doğrula
        ValidateRequest(request);

        // 2. İş kuralı kontrollerini gerçekleştir
        await ValidateBusinessRulesAsync(request, cancellationToken);

        // 3. Alt kategorileri getir
        List<Category> categories = await GetChildCategoriesAsync(request, cancellationToken);

        // 4. Sonuç kontrolü yap
        ValidateResults(categories, request.ParentId);

        // 5. Entity'leri DTO'ya dönüştür ve döndür
        return MapToResponseDtos(categories);
    }

    #endregion

    #region Yardımcı Metotlar (Helper Methods)

    /// <summary>
    /// İstek parametrelerinin geçerliliğini kontrol eder.
    /// </summary>
    /// <param name="request">Doğrulanacak istek.</param>
    /// <exception cref="BusinessException">Geçersiz parametre değerleri için fırlatılır.</exception>
    private static void ValidateRequest(GetChildCategoriesQuery request)
    {
        if (request.ParentId <= 0)
        {
            throw new BusinessException(
                message: "Parent category ID must be greater than 0.",
                userFriendlyMessage: "Geçersiz üst kategori ID'si. Lütfen pozitif bir değer giriniz.",
                errorCode: "INVALID_PARENT_CATEGORY_ID"
            );
        }
    }

    /// <summary>
    /// Alt kategori getirme işlemi öncesi iş kuralı kontrollerini gerçekleştirir.
    /// </summary>
    /// <param name="request">Alt kategori getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task ValidateBusinessRulesAsync(GetChildCategoriesQuery request, CancellationToken cancellationToken)
    {
        // İş kuralı 1: Üst kategori mevcut mu ve aktif mi?
        await _categoryBusinessRules.CheckParentCategoryExistsAsync(request.ParentId, cancellationToken, checkIfActive: true);
    }

    /// <summary>
    /// Alt kategorileri veritabanından getirir.
    /// </summary>
    /// <param name="request">Alt kategori getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Alt kategori entity'lerinin listesi.</returns>
    private async Task<List<Category>> GetChildCategoriesAsync(GetChildCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Yeni specification sistemini kullanarak alt kategorileri getir
            var spec = new CategorySpecifications.Children(request.ParentId, onlyActive: request.OnlyActive);
            return await _categoryRepository.GetListAsync(spec, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new BusinessException(
                message: $"Failed to retrieve child categories for parent ID {request.ParentId}.",
                userFriendlyMessage: "Alt kategoriler getirilirken bir hata oluştu. Lütfen tekrar deneyiniz.",
                errorCode: "CHILD_CATEGORIES_RETRIEVAL_FAILED",
                additionalData: new { ParentId = request.ParentId, OnlyActive = request.OnlyActive, Exception = ex.Message }
            );
        }
    }

    /// <summary>
    /// Sorgu sonuçlarını doğrular ve gerekirse bilgilendirici hatalar fırlatır.
    /// </summary>
    /// <param name="categories">Doğrulanacak kategori listesi.</param>
    /// <param name="parentId">Üst kategori ID'si.</param>
    private static void ValidateResults(List<Category> categories, int parentId)
    {
        if (!categories.Any())
        {
            throw new NotFoundException(
                message: $"No child categories found for parent category with ID {parentId}.",
                userFriendlyMessage: "Bu kategorinin henüz alt kategorisi bulunmuyor.",
                errorCode: "NO_CHILD_CATEGORIES_FOUND"
            );
        }
    }

    /// <summary>
    /// Kategori entity listesini yanıt DTO listesine dönüştürür.
    /// </summary>
    /// <param name="categories">Dönüştürülecek kategori entity'leri.</param>
    /// <returns>Yanıt DTO'larının listesi.</returns>
    private List<GetChildCategoriesResponseDto> MapToResponseDtos(List<Category> categories)
    {
        try
        {
            return _mapper.Map<List<GetChildCategoriesResponseDto>>(categories);
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

    #endregion
}

#endregion