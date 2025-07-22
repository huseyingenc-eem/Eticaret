using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using static Core.Application.Abstractions.Specifications.CommonSpecifications;

namespace ETicaret.Application.Features.Addresses.Specifications;

/// <summary>
/// Address entity'si için özel spesifikasyonlar.
/// Temel ID ve kullanıcı filtreleri için generic specification'ları kullanır.
/// </summary>
public static class AddressSpecifications
{
    /// <summary>
    /// ID'ye göre adres getirme - Generic specification kullanır
    /// </summary>
    public class ById : ByIdSpecification<Address, Guid>
    {
        public ById(Guid id) : base(id) { }
    }

    /// <summary>
    /// ID ve kullanıcı ID'sine göre güvenli adres getirme
    /// </summary>
    public class ByIdAndUserId : Specification<Address>
    {
        public ByIdAndUserId(Guid id, string userId)
            : base(address => address.Id == id && address.UserId == userId)
        {
        }
    }

    /// <summary>
    /// Kullanıcının tüm adreslerini getirme
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
    /// Varsayılan adresleri getirme (teslimat veya fatura)
    /// </summary>
    public class DefaultAddresses : Specification<Address>
    {
        public DefaultAddresses(string userId, bool? isShipping = null, bool? isBilling = null)
            : base(address => address.UserId == userId &&
                   (isShipping == null || address.IsDefaultShipping == isShipping) &&
                   (isBilling == null || address.IsDefaultBilling == isBilling))
        {
        }
    }

    /// <summary>
    /// Sayfalanmış ve filtrelenmiş adres listesi
    /// </summary>
    public class PagedAndFiltered : Specification<Address>
    {
        public PagedAndFiltered(int pageIndex, int pageSize, string? userNameSearch = null, string? cityFilter = null)
            : base(address =>
                (string.IsNullOrEmpty(userNameSearch) || address.User.FirstName.Contains(userNameSearch) || address.User.LastName.Contains(userNameSearch)) &&
                (string.IsNullOrEmpty(cityFilter) || address.City.Contains(cityFilter)))
        {
            AddInclude(a => a.User);
            AddOrderBy(a => a.AddressTitle);
            ApplyPaging(pageIndex * pageSize, pageSize);
        }
    }

    /// <summary>
    /// Kullanıcı ID'si ve adres başlığına göre arama
    /// </summary>
    public class ByUserIdAndTitle : Specification<Address>
    {
        public ByUserIdAndTitle(string userId, string addressTitle)
            : base(address => address.UserId == userId &&
                   address.AddressTitle.ToLower() == addressTitle.ToLower())
        {
        }
    }
}