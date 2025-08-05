using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using static Core.Application.Abstractions.Specifications.CommonSpecifications;

namespace ETicaret.Application.Features.Suppliers.Specifications;

public static class SupplierSpecifications
{
    #region Basic Specifications - Handler'lar için gerekli

    /// <summary>
    /// ID'ye göre tedarikçi getirme - Generic specification kullanır
    /// Rules zaten varlık kontrolü yaptı, Handler sadece get yapar
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

    #endregion

    #region Rule Specifications - Business Rule'lar için kullanılan basit kontroller

    /// <summary>
    /// Şirket adına göre tedarikçi arama
    /// CompanyNameMustBeUniqueRule tarafından kullanılır
    /// </summary>
    public class ByCompanyName : Specification<Supplier>
    {
        public ByCompanyName(string companyName)
            : base(s => s.CompanyName.ToLower() == companyName.ToLower())
        {
        }
    }

    /// <summary>
    /// Şirket adına göre arama (belirtilen ID hariç)
    /// Güncelleme işlemlerinde şirket adı tekrarını kontrol etmek için kullanılır.
    /// CompanyNameMustBeUniqueRule tarafından kullanılır
    /// </summary>
    public class ByCompanyNameExcludingId : Specification<Supplier>
    {
        public ByCompanyNameExcludingId(Guid excludeId, string companyName)
            : base(s => s.Id != excludeId && s.CompanyName.ToLower() == companyName.ToLower())
        {
        }
    }

    /// <summary>
    /// E-posta adresine göre tedarikçi arama
    /// ContactEmailMustBeUniqueRule tarafından kullanılır
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
    /// PhoneNumberMustBeUniqueRule tarafından kullanılır
    /// </summary>
    public class ByPhoneNumber : Specification<Supplier>
    {
        public ByPhoneNumber(string phoneNumber)
            : base(s => s.PhoneNumber != null && s.PhoneNumber == phoneNumber)
        {
        }
    }

    /// <summary>
    /// Aktif ürünlere sahip tedarikçi kontrolü
    /// Tedarikçi silinmeden önce aktif ürünlerinin olup olmadığını kontrol eder.
    /// SupplierCannotHaveActiveProductsRule tarafından kullanılır
    /// </summary>
    public class HasActiveProducts : Specification<Supplier>
    {
        public HasActiveProducts(Guid supplierId)
            : base(s => s.Id == supplierId && s.Products.Any(p => p.IsActive))
        {
            AddInclude(s => s.Products);
        }
    }

    #endregion

    #region Query Specifications - Query handler'lar için karmaşık sorgular

    /// <summary>
    /// Sayfalanmış ve filtrelenmiş tedarikçi listesi
    /// GetListSupplierQueryHandler tarafından kullanılır
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

    /// <summary>
    /// ✅ YENİ: Admin tarafından tedarikçi ara
    /// Admin query'leri için karmaşık filtreleme
    /// </summary>
    public class AdminPagedAndFiltered : Specification<Supplier>
    {
        public AdminPagedAndFiltered(int pageIndex, int pageSize, string? companyNameSearch = null,
            string? contactEmailSearch = null, bool? isActiveFilter = null)
            : base(supplier =>
                (string.IsNullOrEmpty(companyNameSearch) ||
                 supplier.CompanyName.ToLower().Contains(companyNameSearch.ToLower())) &&
                (string.IsNullOrEmpty(contactEmailSearch) ||
                 (supplier.ContactEmail != null && supplier.ContactEmail.ToLower().Contains(contactEmailSearch.ToLower()))) &&
                (isActiveFilter == null || supplier.IsActive == isActiveFilter))
        {
            AddOrderByDescending(s => s.CreatedTime);
            ApplyPaging(pageIndex * pageSize, pageSize);
        }
    }

    /// <summary>
    /// ✅ YENİ: Aktif tedarikçiler ve ürün sayıları
    /// Dashboard veya reports için
    /// </summary>
    public class ActiveWithProductCounts : Specification<Supplier>
    {
        public ActiveWithProductCounts()
            : base(supplier => supplier.IsActive)
        {
            AddInclude(s => s.Products);
            AddOrderBy(s => s.CompanyName);
        }
    }

    #endregion

    #region Optional Search Specifications - Gelişmiş arama özellikleri

    /// <summary>
    /// İletişim kişisine göre arama
    /// </summary>
    public class ByContactPerson : Specification<Supplier>
    {
        public ByContactPerson(string contactPerson)
            : base(s => s.ContactPerson != null && s.ContactPerson.ToLower().Contains(contactPerson.ToLower()))
        {
            AddOrderBy(s => s.ContactPerson);
        }
    }

    /// <summary>
    /// Adres bilgisine göre arama
    /// </summary>
    public class ByAddress : Specification<Supplier>
    {
        public ByAddress(string address)
            : base(s => s.Address != null && s.Address.ToLower().Contains(address.ToLower()))
        {
            AddOrderBy(s => s.CompanyName);
        }
    }

    /// <summary>
    /// Birden fazla kritere göre gelişmiş arama
    /// </summary>
    public class AdvancedSearch : Specification<Supplier>
    {
        public AdvancedSearch(string? companyName = null, string? contactEmail = null,
            string? phoneNumber = null, bool? isActive = null)
            : base(supplier =>
                (string.IsNullOrEmpty(companyName) || supplier.CompanyName.ToLower().Contains(companyName.ToLower())) &&
                (string.IsNullOrEmpty(contactEmail) ||
                 (supplier.ContactEmail != null && supplier.ContactEmail.ToLower().Contains(contactEmail.ToLower()))) &&
                (string.IsNullOrEmpty(phoneNumber) ||
                 (supplier.PhoneNumber != null && supplier.PhoneNumber.Contains(phoneNumber))) &&
                (isActive == null || supplier.IsActive == isActive))
        {
            AddOrderBy(s => s.CompanyName);
        }
    }

    /// <summary>
    /// ✅ YENİ: Ürün sayısına göre tedarikçiler
    /// En çok ürünü olan tedarikçileri bulmak için
    /// </summary>
    public class ByProductCount : Specification<Supplier>
    {
        public ByProductCount(int minProductCount = 1, bool onlyActive = true)
            : base(supplier =>
                (!onlyActive || supplier.IsActive) &&
                supplier.Products.Count >= minProductCount)
        {
            AddInclude(s => s.Products);
            AddOrderByDescending(s => s.Products.Count);
        }
    }

    /// <summary>
    /// ✅ YENİ: Son eklenen tedarikçiler
    /// Dashboard veya recent activities için
    /// </summary>
    public class RecentlyAdded : Specification<Supplier>
    {
        public RecentlyAdded(int days = 30, int take = 10)
            : base(supplier => supplier.CreatedTime >= DateTime.UtcNow.AddDays(-days))
        {
            AddOrderByDescending(s => s.CreatedTime);
            ApplyPaging(0, take);
        }
    }

    #endregion
}