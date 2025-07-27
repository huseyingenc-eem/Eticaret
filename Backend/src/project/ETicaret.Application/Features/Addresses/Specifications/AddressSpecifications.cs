using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using static Core.Application.Abstractions.Specifications.CommonSpecifications;

namespace ETicaret.Application.Features.Addresses.Specifications;

#region Address Specifications Implementation

/// <summary>
/// Address entity'si için veritabanı sorgu spesifikasyonları.
/// Bu sınıf sadece veritabanı sorgu mantığını içerir, iş kuralları içermez.
/// Specification pattern kullanarak karmaşık sorguları yeniden kullanılabilir hale getirir.
/// </summary>
public static class AddressSpecifications
{
    #region Basic ID Specifications

    /// <summary>
    /// ID'ye göre adres getirme - Generic specification kullanır.
    /// </summary>
    public class ById : ByIdSpecification<Address, Guid>
    {
        public ById(Guid id) : base(id) { }
    }

    /// <summary>
    /// ID ve kullanıcı ID'sine göre güvenli adres getirme.
    /// Güvenlik odaklı tasarım - sadece kullanıcının kendi adreslerine erişim.
    /// </summary>
    public class ByIdAndUserId : Specification<Address>
    {
        public ByIdAndUserId(Guid id, string userId)
            : base(address => address.Id == id && address.UserId == userId)
        {
        }
    }

    #endregion

    #region User-Based Specifications

    /// <summary>
    /// Kullanıcının tüm adreslerini getirme.
    /// Sonuçları adres başlığına göre alfabetik olarak sıralar.
    /// </summary>
    public class ByUserId : Specification<Address>
    {
        public ByUserId(string userId)
            : base(address => address.UserId == userId)
        {
            AddOrderBy(a => a.AddressTitle);
        }
    }

    /// <summary>
    /// Kullanıcı ID'si ve adres başlığına göre arama.
    /// Case-insensitive karşılaştırma yapar.
    /// </summary>
    public class ByUserIdAndTitle : Specification<Address>
    {
        public ByUserIdAndTitle(string userId, string addressTitle)
            : base(address => address.UserId == userId &&
                   address.AddressTitle.ToLower() == addressTitle.ToLower())
        {
        }
    }

    /// <summary>
    /// Kullanıcı ID'si ve adres başlığına göre arama (belirtilen ID hariç).
    /// Update işlemlerinde mevcut adresi hariç tutarak duplicate kontrolü yapar.
    /// </summary>
    public class ByUserIdAndTitleExcludingId : Specification<Address>
    {
        public ByUserIdAndTitleExcludingId(string userId, string addressTitle, Guid excludeId)
            : base(address => address.UserId == userId &&
                   address.AddressTitle.ToLower() == addressTitle.ToLower() &&
                   address.Id != excludeId)
        {
        }
    }

    #endregion

    #region Default Address Specifications

    /// <summary>
    /// Varsayılan adresleri getirme (teslimat veya fatura).
    /// Flexible parameter design - hem shipping hem billing adresleri için kullanılabilir.
    /// </summary>
    public class DefaultAddresses : Specification<Address>
    {
        public DefaultAddresses(string userId, bool? isShipping = null, bool? isBilling = null)
            : base(address => address.UserId == userId &&
                   (isShipping == null || address.IsDefaultShipping == isShipping) &&
                   (isBilling == null || address.IsDefaultBilling == isBilling))
        {
            AddOrderBy(a => a.AddressTitle);
        }
    }

    /// <summary>
    /// Kullanıcının varsayılan teslimat adresini getirme.
    /// </summary>
    public class DefaultShippingAddress : Specification<Address>
    {
        public DefaultShippingAddress(string userId)
            : base(address => address.UserId == userId && address.IsDefaultShipping)
        {
        }
    }

    /// <summary>
    /// Kullanıcının varsayılan fatura adresini getirme.
    /// </summary>
    public class DefaultBillingAddress : Specification<Address>
    {
        public DefaultBillingAddress(string userId)
            : base(address => address.UserId == userId && address.IsDefaultBilling)
        {
        }
    }

    #endregion

    #region Search and Filter Specifications

    /// <summary>
    /// Şehir bazında adres arama.
    /// Case-insensitive partial match yapar.
    /// </summary>
    public class ByCity : Specification<Address>
    {
        public ByCity(string city)
            : base(address => address.City.ToLower().Contains(city.ToLower()))
        {
            AddOrderBy(a => a.City);
            AddOrderBy(a => a.AddressTitle);
        }
    }

    /// <summary>
    /// Ülke bazında adres arama.
    /// Case-insensitive exact match yapar.
    /// </summary>
    public class ByCountry : Specification<Address>
    {
        public ByCountry(string country)
            : base(address => address.Country.ToLower() == country.ToLower())
        {
            AddOrderBy(a => a.City);
            AddOrderBy(a => a.AddressTitle);
        }
    }

    #endregion

    #region Admin Specifications

    /// <summary>
    /// Sayfalanmış ve filtrelenmiş adres listesi (Admin için).
    /// Kullanıcı bilgilerini de dahil eder (Eager Loading).
    /// </summary>
    public class PagedAndFiltered : Specification<Address>
    {
        public PagedAndFiltered(int pageIndex, int pageSize, string? userNameSearch = null, string? cityFilter = null)
            : base(address =>
                (string.IsNullOrEmpty(userNameSearch) ||
                 address.User.FirstName.Contains(userNameSearch) ||
                 address.User.LastName.Contains(userNameSearch) ||
                 address.User.Email.Contains(userNameSearch)) &&
                (string.IsNullOrEmpty(cityFilter) || address.City.Contains(cityFilter)))
        {
            AddInclude(a => a.User);
            AddOrderByDescending(a => a.CreatedTime);
            ApplyPaging(pageIndex * pageSize, pageSize);
        }
    }

    /// <summary>
    /// Tüm adresleri sayfalanmış olarak getirme (Admin için).
    /// Kullanıcı bilgilerini de dahil eder.
    /// </summary>
    public class AllPaged : Specification<Address>
    {
        public AllPaged(int pageIndex, int pageSize)
            : base(address => true)
        {
            AddInclude(a => a.User);
            AddOrderByDescending(a => a.CreatedTime);
            ApplyPaging(pageIndex * pageSize, pageSize);
        }
    }

    #endregion

    #region Statistics Specifications

    /// <summary>
    /// Kullanıcının adres sayısını getirme.
    /// Count operation için optimize edilmiş specification.
    /// </summary>
    public class CountByUserId : Specification<Address>
    {
        public CountByUserId(string userId)
            : base(address => address.UserId == userId)
        {
            // Count için include veya ordering gerekli değil
        }
    }

    /// <summary>
    /// Şehir bazında adres istatistikleri.
    /// Count operation için optimize edilmiş specification.
    /// </summary>
    public class CountByCity : Specification<Address>
    {
        public CountByCity(string city)
            : base(address => address.City.ToLower() == city.ToLower())
        {
            // Count için include veya ordering gerekli değil
        }
    }

    #endregion
}

#endregion