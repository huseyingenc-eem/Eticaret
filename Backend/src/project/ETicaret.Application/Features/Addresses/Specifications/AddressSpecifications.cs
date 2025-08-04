using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using static Core.Application.Abstractions.Specifications.CommonSpecifications;

namespace ETicaret.Application.Features.Addresses.Specifications;

/// <summary>
/// Sadece Handler ve Query'ler tarafından kullanılan karmaşık specifications
/// Rule'lar tarafından kullanılan basit specs buradan çıkarıldı ve rule'lara taşındı
/// </summary>
public static class AddressSpecifications
{
    #region Basic Specifications - Handler'lar için gerekli

    /// <summary>
    /// ID'ye göre adres getirme - Handler'larda kullanılır
    /// Rules zaten ownership kontrolü yaptı, Handler sadece get yapar
    /// </summary>
    public class ById : ByIdSpecification<Address, Guid>
    {
        public ById(Guid id) : base(id) { }
    }

    #endregion

    #region Handler Specifications

    /// <summary>
    /// Handler'larda default address management için kullanılır
    /// CreateAddressCommandHandler ve UpdateAddressCommandHandler tarafından kullanılır
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

    #endregion

    #region Query Specifications

    /// <summary>
    /// Query handler'larda kullanılan özel sıralı liste
    /// GetListByUserIdAddressQueryHandler tarafından kullanılır
    /// </summary>
    public class UserAddressesOrdered : Specification<Address>
    {
        public UserAddressesOrdered(string userId)
            : base(address => address.UserId == userId)
        {
            AddOrderBy(a => a.AddressTitle);
        }
    }

    /// <summary>
    /// Admin Query'ler için karmaşık filtreleme
    /// GetListAddressQueryHandler (Admin) tarafından kullanılır
    /// </summary>
    public class AdminPagedAndFiltered : Specification<Address>
    {
        public AdminPagedAndFiltered(int pageIndex, int pageSize, string? userNameSearch = null, string? cityFilter = null)
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
    /// ✅ YENİ: Her kullanıcı için sadece default shipping adresini getiren specification
    /// Admin panelinde kullanıcı başına tek adres göstermek için
    /// </summary>
    public class DefaultShippingPerUser : Specification<Address>
    {
        public DefaultShippingPerUser(int pageIndex, int pageSize, string? userNameSearch = null, string? cityFilter = null)
            : base(address =>
                address.IsDefaultShipping == true && // Sadece default shipping adresleri
                (string.IsNullOrEmpty(userNameSearch) ||
                 address.User.FirstName.Contains(userNameSearch) ||
                 address.User.LastName.Contains(userNameSearch) ||
                 address.User.Email.Contains(userNameSearch)) &&
                (string.IsNullOrEmpty(cityFilter) || address.City.Contains(cityFilter)))
        {
            AddInclude(a => a.User);
            AddOrderBy(a => a.User.FirstName);
            AddOrderBy(a => a.User.LastName);
            ApplyPaging(pageIndex * pageSize, pageSize);
        }
    }

    /// <summary>
    /// ✅ YENİ: Belirli bir kullanıcının tüm adreslerini getiren specification
    /// Kullanıcı detay sayfası için
    /// </summary>
    public class AllAddressesByUser : Specification<Address>
    {
        public AllAddressesByUser(string userId)
            : base(address => address.UserId == userId)
        {
            AddInclude(a => a.User);
            AddOrderByDescending(a => a.IsDefaultShipping);
            AddOrderByDescending(a => a.IsDefaultBilling);
            AddOrderBy(a => a.AddressTitle);
        }
    }

    #endregion

    #region Optional Search Specifications

    /// <summary>
    /// Şehir bazında gelişmiş arama
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
    /// Ülke bazında gelişmiş arama
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
}