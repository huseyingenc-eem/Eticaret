using System.Linq.Expressions;

namespace Core.Application.Abstractions.Specifications;

/// <summary>
/// Belirli bir varlık türü (<typeparamref name="T"/>) için veritabanı sorgusunun şeklini ve davranışını tanımlayan temel arayüz.
/// Bu desen, sorgu mantığını (Where, Include, OrderBy gibi) iş katmanından (Application/Domain) ayırarak,
/// bu mantığı merkezi, test edilebilir ve yeniden kullanılabilir bileşenler haline getirmeyi amaçlar.
/// Entity Framework Core gibi LINQ sağlayıcıları, buradaki Expression ağaçlarını yorumlayarak
/// verimli SQL sorguları oluşturur.
/// </summary>
/// <typeparam name="T">Spesifikasyonun uygulanacağı varlık (entity) türü.</typeparam>
public interface ISpecification<T>
{
    int Skip { get; }
    int Take { get; }
    bool IsPagingEnabled { get; }


    /// <summary>
    /// Sorgu için filtreleme kriterini tanımlar. Bu ifade, SQL'deki 'WHERE' yan tümcesine karşılık gelir.
    /// Örneğin, 'user => user.IsActive && user.RegistrationDate > someDate' gibi bir ifade,
    /// sadece aktif olan ve belirtilen tarihten sonra kaydolmuş kullanıcıları getirecektir.
    /// Bu, spesifikasyonun en temel ve zorunlu parçasıdır.
    /// </summary>
    Expression<Func<T, bool>> Criteria { get; }

    /// <summary>
    /// Sorgu sonucuna dahil edilecek ilişkili varlıkların (navigation property) bir listesini tutar.
    /// Bu, 'Eager Loading' (hevesli yükleme) yapmak için kullanılır ve 'N+1' sorgu problemini önler.
    /// Örneğin, bir Müşteri (Customer) sorgusuna Siparişlerini (Orders) dahil etmek için 'customer => customer.Orders'
    /// ifadesi bu listeye eklenir. Bu, üretilen SQL sorgusuna 'JOIN' eklenmesini sağlar.
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Sonuç kümesini artan düzende (ascending) sıralamak için kullanılacak ifadeyi belirtir.
    /// Bu, SQL'deki 'ORDER BY [Alan] ASC' ifadesine karşılık gelir.
    /// Örneğin, 'product => product.Price' ifadesi ürünleri fiyata göre artan şekilde sıralar.
    /// Yalnızca bir tane 'OrderBy' veya 'OrderByDescending' tanımlanmalıdır.
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Sonuç kümesini azalan düzende (descending) sıralamak için kullanılacak ifadeyi belirtir.
    /// Bu, SQL'deki 'ORDER BY [Alan] DESC' ifadesine karşılık gelir.
    /// Örneğin, 'user => user.CreatedAt' ifadesi kullanıcıları oluşturulma tarihine göre en yeniden eskiye doğru sıralar.
    /// Yalnızca bir tane 'OrderBy' veya 'OrderByDescending' tanımlanmalıdır.
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }
}