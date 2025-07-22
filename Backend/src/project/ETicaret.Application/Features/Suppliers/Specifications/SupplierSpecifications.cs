using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using static Core.Application.Abstractions.Specifications.CommonSpecifications;

namespace ETicaret.Application.Features.Suppliers.Specifications;

/// <summary>
/// Supplier entity'si için spesifikasyonlar.
/// </summary>
public static class SupplierSpecifications
{
    /// <summary>
    /// ID'ye göre tedarikçi getirme - Generic specification kullanır
    /// </summary>
    public class ById : ByIdSpecification<Supplier, Guid>
    {
        public ById(Guid id) : base(id) { }
    }

    /// <summary>
    /// Aktif tedarikçileri getirme - Generic specification kullanır
    /// </summary>
    public class Active : ActiveEntitiesSpecification<Supplier>
    {
        public Active()
        {
            AddOrderBy(s => s.CompanyName);
        }
    }

    /// <summary>
    /// Sayfalanmış aktif tedarikçiler
    /// </summary>
    public class PagedActive : Specification<Supplier>
    {
        public PagedActive(int pageIndex, int pageSize)
            : base(supplier => supplier.IsActive)
        {
            AddOrderBy(s => s.CompanyName);
            ApplyPaging(pageIndex * pageSize, pageSize);
        }
    }

    /// <summary>
    /// Şirket adına göre arama (belirtilen ID hariç)
    /// Güncelleme işlemlerinde şirket adı tekrarını kontrol etmek için kullanılır.
    /// </summary>
    public class ByCompanyNameExcludingId : Specification<Supplier>
    {
        public ByCompanyNameExcludingId(Guid excludeId, string companyName)
            : base(s => s.Id != excludeId && s.CompanyName.ToLower() == companyName.ToLower())
        {
        }
    }

    /// <summary>
    /// Aktif ürünlere sahip tedarikçi kontrolü
    /// Tedarikçi pasifleştirilmeden önce aktif ürünlerinin olup olmadığını kontrol eder.
    /// </summary>
    public class HasActiveProducts : Specification<Supplier>
    {
        public HasActiveProducts(Guid supplierId)
            : base(s => s.Id == supplierId && s.Products.Any(p => p.IsActive))
        {
            AddInclude(s => s.Products);
        }
    }

    /// <summary>
    /// Şirket adına göre tedarikçi arama
    /// </summary>
    public class ByCompanyName : Specification<Supplier>
    {
        public ByCompanyName(string companyName)
            : base(s => s.CompanyName.ToLower() == companyName.ToLower())
        {
        }
    }

    /// <summary>
    /// E-posta adresine göre tedarikçi arama
    /// </summary>
    public class ByContactEmail : Specification<Supplier>
    {
        public ByContactEmail(string email)
            : base(s => s.ContactEmail != null && s.ContactEmail.ToLower() == email.ToLower())
        {
        }
    }

    /// <summary>
    /// Telefon numarasına göre tedarikçi arama
    /// </summary>
    public class ByPhoneNumber : Specification<Supplier>
    {
        public ByPhoneNumber(string phoneNumber)
            : base(s => s.PhoneNumber != null && s.PhoneNumber == phoneNumber)
        {
        }
    }

    /// <summary>
    /// Sayfalanmış ve filtrelenmiş tedarikçi listesi
    /// </summary>
    public class PagedAndFiltered : Specification<Supplier>
    {
        public PagedAndFiltered(int pageIndex, int pageSize, string? companyNameSearch = null, bool onlyActive = true)
            : base(supplier =>
                (!onlyActive || supplier.IsActive) &&
                (string.IsNullOrEmpty(companyNameSearch) || supplier.CompanyName.Contains(companyNameSearch)))
        {
            AddOrderBy(s => s.CompanyName);
            ApplyPaging(pageIndex * pageSize, pageSize);
        }
    }
}