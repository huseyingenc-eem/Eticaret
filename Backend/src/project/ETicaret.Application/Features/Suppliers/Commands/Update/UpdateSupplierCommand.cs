using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Suppliers.Constants;
using ETicaret.Application.Features.Suppliers.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Suppliers.Commands.Update;

#region Komut Sınıfı (Command Class)

/// <summary>
/// Mevcut bir tedarikçiyi güncelleme işlemini temsil eden komut.
/// ITransactionalRequest: Bu işlemin bir transaction içinde çalışmasını sağlar.
/// ICacheRemoverRequest: İşlem başarılı olduğunda ilgili önbelleği temizler.
/// </summary>
public class UpdateSupplierCommand : IRequest<UpdateSupplierResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    #region Özellikler (Properties)
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    #endregion

    #region Önbellek Ayarları (Cache Settings)

    public string? CacheKey => $"supplier:{Id}";
    public bool BypassCache => false;
    public string? CacheGroupKey => SupplierConstants.SuppliersCacheGroup;
    #endregion
}

#endregion

#region Komut İşleyici (Command Handler)

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, UpdateSupplierResponseDto>
{
    #region Alan Tanımlamaları (Field Declarations)

    private readonly IRepository<Supplier, Guid> _supplierRepository;
    private readonly IMapper _mapper;
    #endregion

    #region Yapıcı Metot (Constructor)

    /// <summary>
    /// UpdateSupplierCommandHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemleri için Unit of Work.</param>
    /// <param name="mapper">Entity ve DTO dönüşümleri için AutoMapper.</param>
    public UpdateSupplierCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _supplierRepository = unitOfWork.GetRepository<Supplier, Guid>();
        _mapper = mapper;
    }

    #endregion

    #region İşleme Metotları (Handler Methods)

    /// <summary>
    /// Tedarikçi güncelleme komutunu işler.
    /// </summary>
    /// <param name="request">Güncelleme komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Güncellenen tedarikçi bilgilerini içeren yanıt DTO'su.</returns>
    public async Task<UpdateSupplierResponseDto> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        // 1. Güncellenecek tedarikçiyi veritabanından getir
        Supplier? supplierToUpdate = await GetSupplierByIdAsync(request.Id, cancellationToken);

        // 2. Tedarikçi bulunamadıysa hata fırlat
        ValidateSupplierExists(supplierToUpdate, request.Id);

        // 3. İş kuralı kontrollerini gerçekleştir
        await ValidateBusinessRulesAsync(request, supplierToUpdate!, cancellationToken);

        // 4. Güncelleme işlemini gerçekleştir
        await UpdateSupplierAsync(request, supplierToUpdate!, cancellationToken);

        // 5. Yanıt DTO'sunu oluştur ve döndür
        return CreateResponseDto(supplierToUpdate!);
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
                userFriendlyMessage: "Güncellenmeye çalışılan tedarikçi bulunamadı. Lütfen doğru tedarikçi seçtiğinizden emin olun.",
                errorCode: "SUPPLIER_NOT_FOUND"
            );
        }
    }

    /// <summary>
    /// Güncelleme işlemi öncesi iş kuralı kontrollerini gerçekleştirir.
    /// </summary>
    /// <param name="request">Güncelleme komutu.</param>
    /// <param name="existingSupplier">Mevcut tedarikçi entity'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task ValidateBusinessRulesAsync(UpdateSupplierCommand request, Supplier existingSupplier, CancellationToken cancellationToken)
    {
        // İş kuralı örneği: Tedarikçi adı değiştiriliyorsa ve aynı isimde başka bir tedarikçi varsa hata ver
        if (!string.Equals(existingSupplier.CompanyName, request.CompanyName, StringComparison.OrdinalIgnoreCase))
        {
            await CheckForDuplicateCompanyNameAsync(request.CompanyName, request.Id, cancellationToken);
        }

        // İş kuralı örneği: Pasif yapılmaya çalışılan tedarikçinin aktif ürünleri varsa uyar
        if (existingSupplier.IsActive && !request.IsActive)
        {
            await CheckForActiveProductsAsync(request.Id, cancellationToken);
        }
    }

    /// <summary>
    /// Aynı şirket adına sahip başka bir tedarikçinin varlığını kontrol eder.
    /// </summary>
    /// <param name="companyName">Kontrol edilecek şirket adı.</param>
    /// <param name="excludeId">Kontrol dışında tutulacak tedarikçi ID'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task CheckForDuplicateCompanyNameAsync(string companyName, Guid excludeId, CancellationToken cancellationToken)
    {
        var duplicateCheckSpec = new SupplierSpecifications.ByCompanyNameExcludingId(excludeId, companyName);
        bool isDuplicate = await _supplierRepository.AnyAsync(duplicateCheckSpec, cancellationToken);

        if (isDuplicate)
        {
            throw new BusinessException(
                message: $"A supplier with company name '{companyName}' already exists.",
                userFriendlyMessage: $"'{companyName}' adında başka bir tedarikçi zaten mevcut. Lütfen farklı bir şirket adı seçiniz.",
                errorCode: "DUPLICATE_SUPPLIER_COMPANY_NAME"
            );
        }
    }

    /// <summary>
    /// Tedarikçinin aktif ürünlerinin varlığını kontrol eder.
    /// </summary>
    /// <param name="supplierId">Kontrol edilecek tedarikçinin ID'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task CheckForActiveProductsAsync(Guid supplierId, CancellationToken cancellationToken)
    {
        var activeProductsSpec = new SupplierSpecifications.HasActiveProducts(supplierId);
        bool hasActiveProducts = await _supplierRepository.AnyAsync(activeProductsSpec, cancellationToken);

        if (hasActiveProducts)
        {
            throw new BusinessException(
                message: $"Supplier with ID {supplierId} cannot be deactivated because it has active products.",
                userFriendlyMessage: "Bu tedarikçi pasif hale getirilemez çünkü aktif ürünleri bulunmaktadır. Önce ürünleri pasif hale getirmeniz gerekmektedir.",
                errorCode: "SUPPLIER_HAS_ACTIVE_PRODUCTS"
            );
        }
    }

    /// <summary>
    /// Tedarikçi entity'sini günceller.
    /// </summary>
    /// <param name="request">Güncelleme komutu.</param>
    /// <param name="supplierToUpdate">Güncellenecek tedarikçi entity'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task UpdateSupplierAsync(UpdateSupplierCommand request, Supplier supplierToUpdate, CancellationToken cancellationToken)
    {
        // AutoMapper ile request'ten entity'ye değerleri kopyala
        _mapper.Map(request, supplierToUpdate);

        // Repository'ye güncelleme işlemini bildir
        await _supplierRepository.UpdateAsync(supplierToUpdate, cancellationToken);

        // TransactionBehavior otomatik olarak CompleteAsync() çağrısını yapacak
    }

    /// <summary>
    /// Güncellenen tedarikçi bilgilerinden yanıt DTO'sunu oluşturur.
    /// </summary>
    /// <param name="updatedSupplier">Güncellenen tedarikçi entity'si.</param>
    /// <returns>Yanıt DTO'su.</returns>
    private UpdateSupplierResponseDto CreateResponseDto(Supplier updatedSupplier)
    {
        var response = _mapper.Map<UpdateSupplierResponseDto>(updatedSupplier);
        response.Message = "Tedarikçi bilgileri başarıyla güncellendi.";
        return response;
    }

    #endregion
}

#endregion