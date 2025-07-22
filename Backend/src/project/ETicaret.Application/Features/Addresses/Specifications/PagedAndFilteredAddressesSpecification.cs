using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using System.Linq.Expressions;

namespace ETicaret.Application.Features.Addresses.Specifications;

/// <summary>
/// Adresleri, kullanıcı adı ve şehir bilgisine göre dinamik olarak filtreleyen,
/// sıralayan ve sayfalayan spesifikasyon.
/// </summary>
public class PagedAndFilteredAddressesSpecification : Specification<Address>
{
    /// <summary>
    /// Filtrelenmiş ve sayfalanmış adresleri getiren yeni bir spesifikasyon örneği oluşturur.
    /// </summary>
    /// <param name="pageIndex">Getirilecek sayfanın indeksi (0'dan başlar).</param>
    /// <param name="pageSize">Her sayfadaki kayıt sayısı.</param>
    /// <param name="userNameSearch">Kullanıcı adı, soyadı veya email'de aranacak metin (isteğe bağlı).</param>
    /// <param name="cityFilter">Filtrelenecek şehir adı (isteğe bağlı).</param>
    public PagedAndFilteredAddressesSpecification(int pageIndex, int pageSize, string? userNameSearch, string? cityFilter)
        : base(BuildCriteria(userNameSearch, cityFilter)) // 1. Dinamik olarak filtre kriterini oluştur
    {
        // 2. Sorguya kullanıcı bilgilerini dahil et (Eager Loading)
        AddInclude(a => a.User);

        // 3. Sonuçları oluşturulma tarihine göre en yeniden eskiye sırala
        AddOrderByDescending(a => a.CreatedTime);

        // 4. Sayfalama bilgilerini uygula
        ApplyPaging(pageIndex * pageSize, pageSize);
    }

    /// <summary>
    /// Gelen parametrelere göre dinamik bir filtre ifadesi (Expression) oluşturan yardımcı metot.
    /// </summary>
    private static Expression<Func<Address, bool>>? BuildCriteria(string? userNameSearch, string? cityFilter)
    {
        // LinqKit gibi kütüphanelerle bu işlem daha kolay yapılabilir,
        // ancak temel bir yaklaşımla da dinamik kriter oluşturulabilir.
        // Bu örnekte, basitlik adına, iki filtrenin de aynı anda kullanıldığı varsayılmamıştır.
        // Daha karmaşık senaryolar için bir ExpressionBuilder sınıfı oluşturulabilir.

        if (!string.IsNullOrWhiteSpace(userNameSearch))
        {
            string searchTerm = userNameSearch.ToLower();
            return a => (a.User != null &&
                            (a.User.FirstName.ToLower().Contains(searchTerm) ||
                             a.User.LastName.ToLower().Contains(searchTerm) ||
                             a.User.Email.ToLower().Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(cityFilter))
        {
            return a => a.City.ToLower() == cityFilter.ToLower();
        }

        // Eğer hiçbir filtre yoksa, kriter göndermiyoruz (tüm adresler gelir).
        return null;
    }
}