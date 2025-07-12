using Core.Domain.Entities;
using Core.Application.Interfaces.Paging;
using System.Linq.Expressions;

namespace Core.Application.Interfaces;

/// <summary>
/// Veri erişim katmanı için temel kontratları tanımlayan genel (generic) repository arayüzü.
/// Bu arayüz, CRUD (Create, Read, Update, Delete) operasyonlarını ve yaygın sorgulama desenlerini içerir.
/// Tamamen asenkron bir yapıya sahiptir.
/// </summary>
/// <typeparam name="TEntity">Repository'nin yöneteceği varlık (entity) tipi.</typeparam>
/// <typeparam name="TId">Varlığın birincil anahtar (primary key) tipi.</typeparam>
public interface IRepository<TEntity, TId> where TEntity : Entity<TId>
{
    #region Yazma (Command) Operasyonları

    /// <summary>
    /// Veritabanına yeni bir varlık (entity) asenkron olarak ekler.
    /// Değişikliklerin kalıcı olması için <c>IUnitOfWork.SaveChangesAsync()</c> çağrılmalıdır.
    /// <example>
    /// <code>
    /// var newProduct = new Product { Name = "Yeni Ürün", Price = 150 };
    /// await _productRepository.AddAsync(newProduct);
    /// await _unitOfWork.SaveChangesAsync(); // Değişikliği veritabanına kaydeder.
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="entity">Eklenecek varlık.</param>
    /// <param name="cancellationToken">İşlemin iptal edilmesini sağlayan token.</param>
    /// <returns>Eklenen varlığın takibini sağlayan bir Task.</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Veritabanına birden fazla varlığı asenkron olarak ekler.
    /// <example>
    /// <code>
    /// var products = new List&lt;Product&gt;
    /// {
    ///     new Product { Name = "Ürün A", Price = 100 },
    ///     new Product { Name = "Ürün B", Price = 200 }
    /// };
    /// await _productRepository.AddRangeAsync(products);
    /// await _unitOfWork.SaveChangesAsync();
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="entities">Eklenecek varlık koleksiyonu.</param>
    /// <param name="cancellationToken">İşlemin iptal edilmesini sağlayan token.</param>
    /// <returns>Eklenen varlıkların takibini sağlayan bir Task.</returns>
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mevcut bir varlığı asenkron olarak günceller.
    /// <example>
    /// <code>
    /// var productToUpdate = await _productRepository.GetByIdAsync(1);
    /// if (productToUpdate != null)
    /// {
    ///     productToUpdate.Price = 250;
    ///     await _productRepository.UpdateAsync(productToUpdate);
    ///     await _unitOfWork.SaveChangesAsync();
    /// }
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="entity">Güncellenecek varlık.</param>
    /// <param name="cancellationToken">İşlemin iptal edilmesini sağlayan token.</param>
    /// <returns>Güncellenen varlığın takibini sağlayan bir Task.</returns>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mevcut bir dizi varlığı asenkron olarak günceller.
    /// </summary>
    /// <param name="entities">Güncellenecek varlık koleksiyonu.</param>
    /// <param name="cancellationToken">İşlemin iptal edilmesini sağlayan token.</param>
    /// <returns>Güncellenen varlıkların takibini sağlayan bir Task.</returns>
    Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bir varlığı asenkron olarak siler.
    /// </summary>
    /// <param name="entity">Silinecek varlık.</param>
    /// <param name="permanent">True ise kalıcı (fiziksel) silme, false ise yumuşak (soft) silme yapar.</param>
    /// <param name="cancellationToken">İşlemin iptal edilmesini sağlayan token.</param>
    /// <returns>Silinen varlığın takibini sağlayan bir Task.</returns>
    Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirtilen ID'ye sahip varlığı asenkron olarak siler.
    /// <example>
    /// <code>
    /// // Yumuşak silme (önerilen):
    /// await _productRepository.DeleteByIdAsync(1, permanent: false);
    /// 
    /// // Kalıcı silme (dikkatli kullanılmalı):
    /// await _productRepository.DeleteByIdAsync(2, permanent: true);
    /// 
    /// await _unitOfWork.SaveChangesAsync();
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id">Silinecek varlığın ID'si.</param>
    /// <param name="permanent">True ise kalıcı (fiziksel) silme, false ise yumuşak (soft) silme yapar.</param>
    /// <param name="cancellationToken">İşlemin iptal edilmesini sağlayan token.</param>
    /// <returns>Silinen varlığı veya bulunamazsa null döndüren bir Task.</returns>
    Task<TEntity?> DeleteByIdAsync(TId id, bool permanent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verilen bir dizi varlığı asenkron olarak siler.
    /// <example>
    /// <code>
    /// var productsToDelete = await _productRepository.GetAllAsync(p => p.Stock == 0);
    /// if (productsToDelete.Any())
    /// {
    ///     await _productRepository.DeleteRangeAsync(productsToDelete, permanent: false);
    ///     await _unitOfWork.SaveChangesAsync();
    /// }
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="entities">Silinecek varlık koleksiyonu.</param>
    /// <param name="permanent">True ise kalıcı (fiziksel) silme, false ise yumuşak (soft) silme yapar.</param>
    /// <param name="cancellationToken">İşlemin iptal edilmesini sağlayan token.</param>
    /// <returns>Silinen varlıkların takibini sağlayan bir Task.</returns>
    Task<IEnumerable<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entities, bool permanent = false, CancellationToken cancellationToken = default);

    #endregion

    #region Okuma (Query) Operasyonları

    /// <summary>
    /// Belirtilen ID'ye sahip varlığı, ilişkili alt varlıklarıyla (includes) birlikte asenkron olarak getirir.
    /// <example>
    /// İlişkili verilerle birlikte tek bir kayıt getirme:
    /// <code>
    /// // Ürünü, Kategori ve Marka bilgileriyle birlikte getir.
    /// var product = await _productRepository.GetByIdAsync(
    ///     productId,
    ///     p => p.Category,
    ///     p => p.Brand
    /// );
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id">Getirilecek varlığın ID'si.</param>
    /// <param name="includes">Sorguya dahil edilecek ilişkili varlıkları belirten lambda ifadeleri (örn: x => x.Category).</param>
    /// <returns>Bulunan varlığı veya null döndüren bir Task.</returns>
    Task<TEntity?> GetByIdAsync(TId id, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Belirtilen filtre koşuluna uyan ilk varlığı, ilişkili alt varlıklarıyla (includes) birlikte asenkron olarak getirir.
    /// <example>
    /// <code>
    /// // SKU koduna göre ürünü bulma
    /// var product = await _productRepository.GetAsync(
    ///     p => p.Sku == "ABC-123",
    ///     p => p.Category
    /// );
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="filter">Filtreleme koşulu.</param>
    /// <param name="includes">Sorguya dahil edilecek ilişkili varlıkları belirten lambda ifadeleri.</param>
    /// <returns>Koşula uyan ilk varlığı veya null döndüren bir Task.</returns>
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Belirtilen filtre koşuluna uyan tüm varlıkları, ilişkili alt varlıklarıyla (includes) birlikte bir liste halinde asenkron olarak getirir.
    /// <example>
    /// <code>
    /// // Stoğu 10'dan az olan tüm aktif ürünleri getir.
    /// var lowStockProducts = await _productRepository.GetAllAsync(
    ///     p => p.IsActive &amp;&amp; p.Stock &lt; 10,
    ///     p => p.Brand
    /// );
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="filter">Filtreleme koşulu (opsiyonel).</param>
    /// <param name="includes">Sorguya dahil edilecek ilişkili varlıkları belirten lambda ifadeleri.</param>
    /// <returns>Varlıkların listesini içeren bir Task.</returns>
    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Verileri filtreleyerek, sıralayarak, sayfalayarak ve ilişkili alt varlıklarını da dahil ederek asenkron olarak getirir.
    /// <example>
    /// Filtreli, sıralı ve sayfalı veri listesi getirme:
    /// <code>
    /// var paginatedResult = await _productRepository.GetListAsync(
    ///     filter: p => p.Stock > 0 &amp;&amp; p.IsActive,
    ///     orderBy: q => q.OrderByDescending(p => p.Price),
    ///     index: 0,
    ///     size: 10,
    ///     includes: p => p.Category
    /// );
    /// 
    /// // Dönen sonuçlar kullanılabilir:
    /// foreach(var product in paginatedResult.Items)
    /// {
    ///     Console.WriteLine(product.Name);
    /// }
    /// Console.WriteLine($"Toplam Sayfa: {paginatedResult.Pages}");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="filter">Filtreleme koşulu (opsiyonel).</param>
    /// <param name="orderBy">Sıralama koşulu (opsiyonel).</param>
    /// <param name="index">Getirilecek sayfanın indeksi (0'dan başlar).</param>
    /// <param name="size">Bir sayfadaki kayıt sayısı.</param>
    /// <param name="includes">Sorguya dahil edilecek ilişkili varlıkları belirten lambda ifadeleri.</param>
    /// <returns>Sayfalanmış veri yapısını (`IPaginate`) içeren bir Task.</returns>
    Task<IPaginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, int index = 0, int size = 20, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Belirtilen koşula uyan en az bir varlık olup olmadığını asenkron olarak kontrol eder.
    /// <example>
    /// <code>
    /// bool anyCheapProducts = await _productRepository.AnyAsync(p => p.Price &lt; 10);
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="filter">Filtreleme koşulu (opsiyonel).</param>
    /// <param name="cancellationToken">İşlemin iptal edilmesini sağlayan token.</param>
    /// <returns>Varlık varsa true, yoksa false döndüren bir Task.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirtilen koşula uyan varlıkların toplam sayısını asenkron olarak döndürür.
    /// <example>
    /// <code>
    /// // Stokta olmayan ürünlerin sayısı
    /// int count = await _productRepository.CountAsync(p => p.Stock == 0);
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="filter">Filtreleme koşulu (opsiyonel).</param>
    /// <param name="cancellationToken">İşlemin iptal edilmesini sağlayan token.</param>
    /// <returns>Toplam kayıt sayısını içeren bir Task.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default);

    #endregion

    #region Gelişmiş Sorgulama

    /// <summary>
    /// Standart metotların yetersiz kaldığı karmaşık ve özel sorgu senaryoları için
    /// veritabanı sorgu nesnesini (`IQueryable`) doğrudan erişime açar.
    /// Not: Bu metot, soyutlama katmanını (abstraction layer) delebileceği için dikkatli ve
    /// sadece zorunlu durumlarda kullanılmalıdır.
    /// <example>
    /// <code>
    /// // DİKKAT: Bu kullanım, Application katmanını Infrastructure detaylarına (EF Core)
    /// // bağımlı hale getirebilir ve sadece özel durumlarda düşünülmelidir.
    /// var complexQuery = _repository.Query()
    ///     .Where(p => p.Name.StartsWith("A"))
    ///     .GroupBy(p => p.CategoryId)
    ///     .Select(g => new { CategoryId = g.Key, Count = g.Count() });
    /// </code>
    /// </example>
    /// </summary>
    /// <returns>Sorgulanabilir bir `IQueryable&lt;TEntity&gt;` nesnesi.</returns>
    IQueryable<TEntity> Query();

    #endregion
}