using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Suppliers.Constants;
using ETicaret.Application.Features.Suppliers.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Suppliers.Queries.GetById;

#region Sorgu Sınıfı (Query Class)

/// <summary>
/// Belirtilen ID'ye sahip tedarikçiyi getiren sorgu.
/// ICachableRequest: Bu sorgunun sonucunun önbelleğe alınmasını sağlar.
/// Önbellek sayesinde aynı tedarikçi bilgisi tekrar sorgulandığında veritabanına gitmeden hızlı yanıt alınır.
/// </summary>
public class GetByIdSupplierQuery : IRequest<GetByIdSupplierResponseDto>, ICachableRequest
{
    #region Özellikler (Properties)
    public Guid Id { get; set; }

    #endregion

    #region Önbellek Ayarları (Cache Settings)

    /// <summary>
    /// Önbelleği atlayıp doğrudan veritabanından veri çekip çekmeyeceğini belirtir.
    /// False: Önbellek kullanılır, True: Doğrudan veritabanından çekilir.
    /// </summary>
    public bool BypassCache { get; set; }

    /// <summary>
    /// Bu tedarikçiye özgü önbellek anahtarı.
    /// Formatı: "supplier:{TedarikçiId}"
    /// </summary>
    public string CacheKey => $"supplier:{Id}";

    /// <summary>
    /// Tedarikçiler ile ilgili önbellek grubu anahtarı.
    /// Bu grup altındaki tüm önbellek verileri toplu olarak temizlenebilir.
    /// </summary>
    public string? CacheGroupKey => SupplierConstants.SuppliersCacheGroup;

    /// <summary>
    /// Kayan son kullanma süresi. Belirtilen süre boyunca erişilmezse önbellekten kaldırılır.
    /// Null ise varsayılan süre (CacheSettings'ten) kullanılır.
    /// </summary>
    public TimeSpan? SlidingExpiration { get; set; }

    /// <summary>
    /// Mutlak son kullanma süresi. Önbelleğe eklendikten 1 saat sonra otomatik olarak temizlenir.
    /// Bu, veri güncelliğini garanti etmek için kullanılır.
    /// </summary>
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(1);

    #endregion
}

#endregion

#region Sorgu İşleyici (Query Handler)

public class GetByIdSupplierQueryHandler : IRequestHandler<GetByIdSupplierQuery, GetByIdSupplierResponseDto>
{
    #region Alan Tanımlamaları (Field Declarations)

    private readonly IRepository<Supplier, Guid> _supplierRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Yapıcı Metot (Constructor)
    public GetByIdSupplierQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        // Generic repository'yi Unit of Work üzerinden alıyoruz
        _supplierRepository = unitOfWork.GetRepository<Supplier, Guid>();
        _mapper = mapper;
    }

    #endregion

    #region İşleme Metotları (Handler Methods)

    /// <summary>
    /// Tedarikçi getirme sorgusunu işler.
    /// </summary>
    /// <param name="request">Tedarikçi getirme sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Tedarikçi bilgilerini içeren yanıt DTO'su.</returns>
    /// <exception cref="NotFoundException">Tedarikçi bulunamadığında fırlatılır.</exception>
    public async Task<GetByIdSupplierResponseDto> Handle(GetByIdSupplierQuery request, CancellationToken cancellationToken)
    {
        // 1. Belirtilen ID'ye sahip tedarikçiyi veritabanından getir
        Supplier? supplier = await GetSupplierByIdAsync(request.Id, cancellationToken);

        // 2. Tedarikçi bulunamadıysa hata fırlat
        ValidateSupplierExists(supplier, request.Id);

        // 3. Entity'yi DTO'ya dönüştür ve döndür
        return _mapper.Map<GetByIdSupplierResponseDto>(supplier);
    }

    #endregion

    #region Yardımcı Metotlar (Helper Methods)

    /// <summary>
    /// Belirtilen ID'ye sahip tedarikçiyi veritabanından getirir.
    /// </summary>
    /// <param name="supplierId">Tedarikçinin benzersiz kimliği.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Bulunan tedarikçi entity'si veya null.</returns>
    private async Task<Supplier?> GetSupplierByIdAsync(Guid supplierId, CancellationToken cancellationToken)
    {
        // Yeni generic specification sistemini kullanarak tedarikçiyi getir
        var spec = new SupplierSpecifications.ById(supplierId);
        return await _supplierRepository.GetAsync(spec, cancellationToken);
    }

    /// <summary>
    /// Tedarikçinin varlığını doğrular, bulunamazsa hata fırlatır.
    /// </summary>
    /// <param name="supplier">Doğrulanacak tedarikçi entity'si.</param>
    /// <param name="supplierId">Aranan tedarikçinin ID'si.</param>
    /// <exception cref="NotFoundException">Tedarikçi bulunamadığında fırlatılır.</exception>
    private static void ValidateSupplierExists(Supplier? supplier, Guid supplierId)
    {
        if (supplier == null)
        {
            throw new NotFoundException(
                message: $"Supplier with ID {supplierId} was not found in the database.",
                userFriendlyMessage: "Aranan tedarikçi bulunamadı. Lütfen doğru tedarikçi seçtiğinizden emin olun.",
                errorCode: "SUPPLIER_NOT_FOUND"
            );
        }
    }
    #endregion
}

#endregion