using AutoMapper;
using Core.Application.Abstractions.Paging;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Results;
using ETicaret.Application.Features.Suppliers.Constants;
using ETicaret.Application.Features.Suppliers.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Suppliers.Queries.GetList;

#region Sorgu Sınıfı (Query Class)

/// <summary>
/// Tedarikçileri sayfalanmış bir liste olarak getiren sorgu.
/// ICachableRequest: Bu sorgunun sonucunun önbelleğe alınmasını sağlar.
/// Filtreleme ve sayfalama desteği ile yüksek performans sunar.
/// </summary>
public class GetListSupplierQuery : IRequest<PagedResult<GetListSupplierResponseDto>>, ICachableRequest
{
    #region Özellikler (Properties)
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;

    public string? CompanyNameSearch { get; set; }
    public bool OnlyActive { get; set; } = true;

    #endregion

    #region Önbellek Ayarları (Cache Settings)

    public bool BypassCache { get; set; }

    public string CacheKey => $"supplier-list_page_{PageIndex}_size_{PageSize}_company_{CompanyNameSearch}_active_{OnlyActive}";
    public string? CacheGroupKey => SupplierConstants.SuppliersCacheGroup;

    public TimeSpan? SlidingExpiration { get; set; }

    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(1);

    #endregion
}

#endregion

#region Sorgu İşleyici (Query Handler)

/// <summary>
/// GetListSupplierQuery sorgusunu işleyen handler sınıfı.
/// Sayfalama, filtreleme, önbellekleme ve hata yönetimi ile kapsamlı liste getirme işlemi.
/// </summary>
public class GetListSupplierQueryHandler : IRequestHandler<GetListSupplierQuery, PagedResult<GetListSupplierResponseDto>>
{
    #region Alan Tanımlamaları (Field Declarations)

    private readonly IRepository<Supplier, Guid> _supplierRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Yapıcı Metot (Constructor)
    public GetListSupplierQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _supplierRepository = unitOfWork.GetRepository<Supplier, Guid>();
        _mapper = mapper;
    }

    #endregion

    #region İşleme Metotları (Handler Methods)

    public async Task<PagedResult<GetListSupplierResponseDto>> Handle(GetListSupplierQuery request, CancellationToken cancellationToken)
    {
        // 1. İstek parametrelerini doğrula
        ValidateRequest(request);

        // 2. Filtrelenmiş ve sayfalanmış tedarikçileri getir
        IPaginate<Supplier> suppliersPage = await GetFilteredSuppliersAsync(request, cancellationToken);

        // 3. İş kuralı kontrolü: Hiç tedarikçi yoksa bilgilendirici hata
        ValidateResults(suppliersPage, request);

        // 4. Entity'leri DTO'ya dönüştür ve sayfalama bilgilerini koru
        return CreatePagedResponse(suppliersPage);
    }

    #endregion

    #region Yardımcı Metotlar (Helper Methods)

    /// <summary>
    /// İstek parametrelerinin geçerliliğini kontrol eder.
    /// </summary>
    /// <param name="request">Doğrulanacak istek.</param>
    /// <exception cref="BusinessException">Geçersiz parametre değerleri için fırlatılır.</exception>
    private static void ValidateRequest(GetListSupplierQuery request)
    {
        // Sayfa indeksi negatif olamaz
        if (request.PageIndex < 0)
        {
            throw new BusinessException(
                message: "Page index cannot be negative.",
                userFriendlyMessage: "Sayfa numarası geçersiz. Lütfen 0 veya daha büyük bir değer giriniz.",
                errorCode: "INVALID_PAGE_INDEX"
            );
        }

        // Sayfa boyutu 1-100 arasında olmalı
        if (request.PageSize <= 0)
        {
            throw new BusinessException(
                message: "Page size must be greater than 0.",
                userFriendlyMessage: "Sayfa boyutu 1'den küçük olamaz.",
                errorCode: "INVALID_PAGE_SIZE_MIN"
            );
        }

        if (request.PageSize > 100)
        {
            throw new BusinessException(
                message: "Page size cannot exceed 100.",
                userFriendlyMessage: "Sayfa boyutu 100'den büyük olamaz. Performans için daha küçük bir değer seçiniz.",
                errorCode: "INVALID_PAGE_SIZE_MAX"
            );
        }

        // Arama terimi çok kısa ise uyar
        if (!string.IsNullOrEmpty(request.CompanyNameSearch) && request.CompanyNameSearch.Trim().Length < 2)
        {
            throw new BusinessException(
                message: "Company name search term must be at least 2 characters.",
                userFriendlyMessage: "Şirket adı araması en az 2 karakter olmalıdır.",
                errorCode: "SEARCH_TERM_TOO_SHORT"
            );
        }
    }

    /// <summary>
    /// Filtrelenmiş ve sayfalanmış tedarikçileri veritabanından getirir.
    /// </summary>
    /// <param name="request">Filtreleme ve sayfalama parametreleri.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Sayfalanmış tedarikçi listesi.</returns>
    private async Task<IPaginate<Supplier>> GetFilteredSuppliersAsync(GetListSupplierQuery request, CancellationToken cancellationToken)
    {
        // Dinamik filtreleme ve sayfalama specification'ı oluştur
        var spec = new SupplierSpecifications.PagedAndFiltered(
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            companyNameSearch: request.CompanyNameSearch?.Trim(),
            onlyActive: request.OnlyActive
        );

        return await _supplierRepository.GetPaginatedListAsync(spec, cancellationToken);
    }

    /// <summary>
    /// Sorgu sonuçlarını doğrular ve gerekirse bilgilendirici hatalar fırlatır.
    /// </summary>
    /// <param name="suppliersPage">Doğrulanacak sonuçlar.</param>
    /// <param name="request">Orijinal istek parametreleri.</param>
    private static void ValidateResults(IPaginate<Supplier> suppliersPage, GetListSupplierQuery request)
    {
        // Hiç tedarikçi yoksa (ilk sayfa ve toplam 0)
        if (suppliersPage.Count == 0 && request.PageIndex == 0)
        {
            if (!string.IsNullOrEmpty(request.CompanyNameSearch))
            {
                throw new NotFoundException(
                    message: $"No suppliers found with company name containing '{request.CompanyNameSearch}'.",
                    userFriendlyMessage: $"'{request.CompanyNameSearch}' içeren şirket adıyla hiç tedarikçi bulunamadı.",
                    errorCode: "NO_SUPPLIERS_FOUND_SEARCH"
                );
            }
            else if (request.OnlyActive)
            {
                throw new NotFoundException(
                    message: "No active suppliers found in the system.",
                    userFriendlyMessage: "Sistemde hiç aktif tedarikçi bulunamadı.",
                    errorCode: "NO_ACTIVE_SUPPLIERS_FOUND"
                );
            }
            else
            {
                throw new NotFoundException(
                    message: "No suppliers found in the system.",
                    userFriendlyMessage: "Sistemde hiç tedarikçi bulunamadı.",
                    errorCode: "NO_SUPPLIERS_FOUND"
                );
            }
        }

        // İstenen sayfa mevcut olmayan bir sayfa ise
        if (request.PageIndex >= suppliersPage.Pages && suppliersPage.Count > 0)
        {
            throw new BusinessException(
                message: $"Requested page {request.PageIndex} does not exist. Total pages: {suppliersPage.Pages}.",
                userFriendlyMessage: $"İstenen sayfa ({request.PageIndex + 1}) mevcut değil. Toplam sayfa sayısı: {suppliersPage.Pages}.",
                errorCode: "PAGE_NOT_EXISTS"
            );
        }
    }

    /// <summary>
    /// Entity listesini DTO listesine dönüştürür ve sayfalama bilgilerini korur.
    /// </summary>
    /// <param name="suppliersPage">Dönüştürülecek sayfalanmış entity listesi.</param>
    /// <returns>Sayfalanmış DTO yanıtı.</returns>
    private PagedResult<GetListSupplierResponseDto> CreatePagedResponse(IPaginate<Supplier> suppliersPage)
    {
        // Entity'leri DTO'ya dönüştür
        List<GetListSupplierResponseDto> supplierDtos = _mapper.Map<List<GetListSupplierResponseDto>>(suppliersPage.Items);

        // Sayfalama bilgilerini koruyarak yeni PagedResult oluştur
        return new PagedResult<GetListSupplierResponseDto>(
            items: supplierDtos,
            count: suppliersPage.Count,
            index: suppliersPage.Index,
            size: suppliersPage.Size
        );
    }

    #endregion
}

#endregion