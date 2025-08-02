using Core.Application.Abstractions.Paging;
using Core.Application.Abstractions.Specifications;
using Core.Domain.Entities;

namespace Core.Application.Abstractions.Repositories;

/// <summary>
/// Tüm varlık türleri için temel veri erişim operasyonlarını tanımlayan jenerik repository arayüzü.
/// Bu arayüz, Clean Architecture'da Application ve Infrastructure katmanları arasında bir soyutlama katmanı görevi görür.
/// Sorgu mantığı, yeniden kullanılabilir ve test edilebilir bir yapı sunan Specification deseni ile yönetilir.
/// </summary>
/// <typeparam name="TEntity">Repository'nin yöneteceği, IEntity arayüzünü uygulayan varlık türü.</typeparam>
/// <typeparam name="TId">Varlığın kimlik (ID) türü (Örn: Guid, int).</typeparam>
public interface IRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    #region Yazma Operasyonları (Write Operations)

    /// <summary>
    /// Yeni bir varlığı veritabanına asenkron olarak eklenmek üzere işaretler.
    /// Değişiklikler, UnitOfWork kapsamında SaveChangesAsync çağrıldığında veritabanına yansıtılır.
    /// </summary>
    /// <param name="entity">Eklenecek varlık.</param>
    /// <param name="cancellationToken">Operasyonun iptal edilmesini sağlayan token.</param>
    /// <returns>Eklenen varlığın kendisini döndüren bir Task.</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Birden fazla yeni varlığı toplu olarak veritabanına asenkron olarak eklenmek üzere işaretler.
    /// Bu, tek tek eklemekten daha performanslıdır.
    /// </summary>
    /// <param name="entities">Eklenecek varlıkların listesi.</param>
    /// <param name="cancellationToken">Operasyonun iptal edilmesini sağlayan token.</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mevcut bir varlığın durumunu güncellenmiş olarak asenkron bir şekilde işaretler.
    /// </summary>
    /// <param name="entity">Güncellenecek varlık.</param>
    /// <param name="cancellationToken">Operasyonun iptal edilmesini sağlayan token.</param>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);


    /// <summary>
    /// Birden fazla mevcut varlığın durumunu güncellenmiş olarak asenkron bir şekilde işaretler.
    /// Bu, tek tek güncellemekten daha performanslıdır.
    /// </summary>
    /// <param name="entities">Güncellenecek varlıkların listesi.</param>
    /// <param name="cancellationToken">Operasyonun iptal edilmesini sağlayan token.</param>
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mevcut bir varlığı silinmiş olarak asenkron bir şekilde işaretler.
    /// </summary>
    /// <param name="entity">Silinecek varlık.</param>
    /// <param name="permanent">
    /// Varlığın kalıcı olarak mı (hard delete) yoksa geçici olarak mı (soft delete) silineceğini belirtir.
    /// False ise ve varlık ISoftDeletable arayüzünü uyguluyorsa, DeletedTime alanı doldurulur.
    /// </param>
    /// <param name="cancellationToken">Operasyonun iptal edilmesini sağlayan token.</param>
    Task DeleteAsync(TEntity entity, bool permanent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirtilen kimliğe (ID) sahip varlığı bularak asenkron olarak siler.
    /// </summary>
    /// <param name="id">Silinecek varlığın kimliği.</param>
    /// <param name="permanent">Kalıcı (hard delete) veya geçici (soft delete) silme tercihi.</param>
    /// <param name="cancellationToken">Operasyonun iptal edilmesini sağlayan token.</param>
    Task DeleteByIdAsync(TId id, bool permanent = false, CancellationToken cancellationToken = default);

    #endregion

    #region Okuma Operasyonları (Read Operations)

    /// <summary>
    /// Verilen spesifikasyona uyan tek bir varlığı asenkron olarak getirir.
    /// Birden fazla sonuç bulunursa ilkini, hiç bulunmazsa null döner.
    /// </summary>
    /// <param name="spec">Filtre, sıralama ve include bilgilerini içeren spesifikasyon.</param>
    /// <param name="cancellationToken">Operasyonun iptal edilmesini sağlayan token.</param>
    /// <returns>Bulunan varlık veya null.</returns>
    Task<TEntity?> GetAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verilen spesifikasyona uyan tüm varlıkların listesini asenkron olarak getirir.
    /// </summary>
    /// <param name="spec">Filtre, sıralama ve include bilgilerini içeren spesifikasyon.</param>
    /// <param name="cancellationToken">Operasyonun iptal edilmesini sağlayan token.</param>
    /// <returns>Sonuçları içeren bir liste. Sonuç yoksa boş liste döner.</returns>
    Task<List<TEntity>> GetListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verilen spesifikasyona uyan varlıkları sayfalanmış bir yapıda asenkron olarak getirir.
    /// </summary>
    /// <param name="spec">Sayfalama dahil filtre, sıralama ve include bilgilerini içeren spesifikasyon.</param>
    /// <param name="cancellationToken">Operasyonun iptal edilmesini sağlayan token.</param>
    /// <returns>Sayfalama bilgilerini ve sonuç listesini içeren bir IPaginate nesnesi.</returns>
    Task<IPaginate<TEntity>> GetPaginatedListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verilen spesifikasyona uyan varlık sayısını asenkron olarak sayar.
    /// </summary>
    /// <param name="spec">Filtre koşullarını içeren spesifikasyon (isteğe bağlı).</param>
    /// <param name="cancellationToken">Operasyonun iptal edilmesini sağlayan token.</param>
    /// <returns>Toplam kayıt sayısı.</returns>
    Task<int> CountAsync(ISpecification<TEntity>? spec = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verilen spesifikasyona uyan en az bir varlık olup olmadığını asenkron olarak kontrol eder.
    /// Bu, CountAsync() > 0'dan daha performanslıdır.
    /// </summary>
    /// <param name="spec">Filtre koşullarını içeren spesifikasyon (isteğe bağlı).</param>
    /// <param name="cancellationToken">Operasyonun iptal edilmesini sağlayan token.</param>
    /// <returns>Eşleşen kayıt varsa true, yoksa false.</returns>
    Task<bool> AnyAsync(ISpecification<TEntity>? spec = null, CancellationToken cancellationToken = default);

    #endregion
}