using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using ETicaret.Application.Features.Categories.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Queries.GetParentCategories;

#region Sorgu Sınıfı (Query Class)

/// <summary>
/// Sadece en üst seviyedeki (ana/parent) kategorileri getirmek için kullanılan sorgu.
/// ICachableRequest: Bu sorgunun sonucunun önbelleğe alınmasını sağlar.
/// Ana kategoriler sık değişmediği için önbellekleme uygun bir performans optimizasyonudur.
/// </summary>
public class GetParentCategoriesQuery : IRequest<List<GetParentCategoriesResponseDto>>, ICachableRequest
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
    private readonly IRepository<Category, int> _categoryRepository;

    #endregion

    #region Yapıcı Metot (Constructor)
    public GetParentCategoriesQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _categoryRepository = unitOfWork.GetRepository<Category, int>();
    }

    #endregion

    #region İşleme Metotları (Handler Methods)

    /// <summary>
    /// Ana kategorileri getirme sorgusunu işler.
    /// </summary>
    /// <param name="request">Ana kategori getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Ana kategorilerin listesini içeren yanıt DTO'su.</returns>
    public async Task<List<GetParentCategoriesResponseDto>> Handle(GetParentCategoriesQuery request, CancellationToken cancellationToken)
    {
        // 1. Ana kategorileri (ParentId'si null olanlar) getirmek için specification oluştur
        var categories = await GetParentCategoriesAsync(cancellationToken);

        // 2. Entity listesini DTO listesine dönüştür
        return MapToResponseDtos(categories);
    }

    #endregion

    #region Yardımcı Metotlar (Helper Methods)

    /// <summary>
    /// Ana kategorileri veritabanından getirir.
    /// Ana kategoriler: ParentId'si null olan kategorilerdir.
    /// </summary>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Ana kategori entity'lerinin listesi.</returns>
    private async Task<List<Category>> GetParentCategoriesAsync(CancellationToken cancellationToken)
    {
        // Yeni specification sistemini kullanarak ana kategorileri getir
        var spec = new CategorySpecifications.Parents();
        return await _categoryRepository.GetListAsync(spec, cancellationToken);
    }

    /// <summary>
    /// Kategori entity listesini yanıt DTO listesine dönüştürür.
    /// </summary>
    /// <param name="categories">Dönüştürülecek kategori entity'leri.</param>
    /// <returns>Yanıt DTO'larının listesi.</returns>
    private List<GetParentCategoriesResponseDto> MapToResponseDtos(List<Category> categories)
    {
        return _mapper.Map<List<GetParentCategoriesResponseDto>>(categories);
    }

    #endregion
}

#endregion