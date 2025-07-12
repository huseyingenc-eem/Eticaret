using Core.Domain.Entities;
using Core.Application.Interfaces;
using Core.Application.Interfaces.Paging;
using Core.Infrastructure.Persistence.Repositories.Ef; // ToPaginateAsync için

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// IRepository arayüzünün Entity Framework Core kullanarak somut (concrete) implementasyonu.
/// Veritabanı işlemleri, denetim (auditing), yumuşak silme (soft delete) ve
/// ilişkisel veri yönetimi gibi tüm altyapı mantıkları bu sınıfta yer alır.
/// </summary>
public class EfRepositoryBase<TEntity, TId, TContext> : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : IEquatable<TId>
    where TContext : DbContext
{
    protected readonly TContext Context;

    public EfRepositoryBase(TContext context)
    {
        Context = context;
    }

    /// <summary>
    /// Sorgu işlemlerinin temelini oluşturan ve yumuşak silinmiş kayıtları otomatik olarak
    /// filtreleyen bir IQueryable nesnesi döndürür. Okuma operasyonlarının varsayılan başlangıç noktasıdır.
    /// </summary>
    /// <returns>Yumuşak silinmemiş varlıklar için sorgulanabilir bir nesne.</returns>
    public virtual IQueryable<TEntity> Query() => Context.Set<TEntity>().Where(e => e.DeletedTime == null);

    #region Yazma (Command) Operasyonları

    /// <summary>
    /// Yeni bir varlığı veritabanına eklemek üzere işaretler.
    /// <c>CreatedTime</c> alanını otomatik olarak mevcut UTC zamanına ayarlar.
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
    /// <param name="cancellationToken">İşlem iptal token'ı.</param>
    /// <returns>Eklenen varlık.</returns>
    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedTime = DateTime.UtcNow;
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <summary>
    /// Birden fazla yeni varlığı toplu olarak veritabanına eklemek üzere işaretler.
    /// Her bir varlığın <c>CreatedTime</c> alanını otomatik olarak ayarlar.
    /// </summary>
    /// <param name="entities">Eklenecek varlıkların koleksiyonu.</param>
    /// <param name="cancellationToken">İşlem iptal token'ı.</param>
    /// <returns>Eklenen varlıkların koleksiyonu.</returns>
    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
            entity.CreatedTime = DateTime.UtcNow;

        await Context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    /// <summary>
    /// Mevcut bir varlığı günceller.
    /// Güvenli bir yöntemdir: Önce varlığı ID'si ile veritabanından bulur, ardından
    /// gelen yeni verileri mevcut varlığa uygular ve <c>UpdateTime</c> alanını günceller.
    /// <example>
    /// <code>
    /// var productToUpdate = await _productRepository.GetByIdAsync(1); // Takip edilmeyen bir sorgu olabilir.
    /// if (productToUpdate != null)
    /// {
    ///     productToUpdate.Price = 200; // DTO'dan gelen yeni değerler atanır.
    ///     await _productRepository.UpdateAsync(productToUpdate);
    ///     await _unitOfWork.SaveChangesAsync();
    /// }
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="entity">Güncellenecek verileri içeren varlık.</param>
    /// <param name="cancellationToken">İşlem iptal token'ı.</param>
    /// <returns>Güncellenen varlık.</returns>
    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var existingEntity = await Context.Set<TEntity>().FindAsync(new object[] { entity.Id }, cancellationToken);
        if (existingEntity != null)
        {
            Context.Entry(existingEntity).CurrentValues.SetValues(entity);
            existingEntity.UpdateTime = DateTime.UtcNow;
        }
        return entity;
    }

    /// <summary>
    /// Birden fazla varlığı toplu olarak günceller.
    /// Her bir varlığın <c>UpdateTime</c> alanını otomatik olarak ayarlar.
    /// </summary>
    /// <param name="entities">Güncellenecek varlıkların koleksiyonu.</param>
    /// <param name="cancellationToken">İşlem iptal token'ı.</param>
    /// <returns>Güncellenen varlıkların koleksiyonu.</returns>
    public virtual Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
            entity.UpdateTime = DateTime.UtcNow;

        Context.Set<TEntity>().UpdateRange(entities);
        return Task.FromResult(entities);
    }

    /// <summary>
    /// Bir varlığı silmek üzere işaretler.
    /// </summary>
    /// <param name="entity">Silinecek varlık.</param>
    /// <param name="permanent">Eğer <c>true</c> ise veritabanından fiziksel olarak siler.
    /// Eğer <c>false</c> ise <c>DeletedTime</c> alanını güncelleyerek yumuşak silme (soft delete) yapar.</param>
    /// <param name="cancellationToken">İşlem iptal token'ı.</param>
    /// <returns>İşlem görmüş varlık.</returns>
    public virtual Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false, CancellationToken cancellationToken = default)
    {
        if (!permanent)
        {
            entity.DeletedTime = DateTime.UtcNow;
            entity.UpdateTime = DateTime.UtcNow;
            Context.Entry(entity).State = EntityState.Modified;
        }
        else
        {
            Context.Entry(entity).State = EntityState.Deleted;
        }
        return Task.FromResult(entity);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip varlığı bularak siler.
    /// <example>
    /// <code>
    /// Yumuşak silme (önerilen)
    /// await _productRepository.DeleteByIdAsync(1, permanent: false);
    /// 
    /// Kalıcı silme (dikkatli kullanılmalı)
    /// await _productRepository.DeleteByIdAsync(2, permanent: true);
    /// 
    /// await _unitOfWork.SaveChangesAsync();
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id">Silinecek varlığın ID'si.</param>
    /// <param name="permanent">Yumuşak (false) veya kalıcı (true) silme seçeneği.</param>
    /// <param name="cancellationToken">İşlem iptal token'ı.</param>
    /// <returns>Silinen varlık veya bulunamazsa <c>null</c>.</returns>
    public virtual async Task<TEntity?> DeleteByIdAsync(TId id, bool permanent = false, CancellationToken cancellationToken = default)
    {
        var entity = await Query().FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        if (entity == null) return null;

        await DeleteAsync(entity, permanent, cancellationToken);
        return entity;
    }

    /// <summary>
    /// Birden fazla varlığı toplu olarak silmek üzere işaretler.
    /// 
    /// <example>
    /// <code>
    /// Yumuşak silme (önerilen)
    /// await _productRepository.DeleteRangeAsync(1, permanent: false);
    /// 
    /// Kalıcı silme (dikkatli kullanılmalı)
    /// await _productRepository.DeleteRangeAsync(2, permanent: true);
    /// 
    /// await _unitOfWork.SaveChangesAsync();
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="entities">Silinecek varlıkların koleksiyonu.</param>
    /// <param name="permanent">Yumuşak (false) veya kalıcı (true) silme seçeneği.</param>
    /// <param name="cancellationToken">İşlem iptal token'ı.</param>
    /// <returns>İşlem görmüş varlıkların koleksiyonu.</returns>
    public virtual Task<IEnumerable<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entities, bool permanent = false, CancellationToken cancellationToken = default)
    {
        if (!permanent)
        {
            foreach (var entity in entities)
            {
                entity.DeletedTime = DateTime.UtcNow;
                entity.UpdateTime = DateTime.UtcNow;
                Context.Entry(entity).State = EntityState.Modified;
            }
        }
        else
        {
            Context.Set<TEntity>().RemoveRange(entities);
        }
        return Task.FromResult(entities);
    }
    #endregion

    #region Okuma (Query) Operasyonları
    /// <summary>
    /// ID'ye göre bir varlığı, belirtilen ilişkili verileriyle (includes) birlikte getirir.
    /// Okuma performansı için <c>AsNoTracking()</c> metodu varsayılan olarak kullanılır.
    /// <example>
    /// İlişkili verilerle birlikte tek bir kayıt getirme:
    /// <code>
    /// Ürünü, Kategori ve Marka bilgileriyle birlikte getir.
    /// var product = await _productRepository.GetByIdAsync(
    ///     productId,
    ///     p => p.Category,
    ///     p => p.Brand
    /// );
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id">Aranan varlığın ID'si.</param>
    /// <param name="includes">Sorguya dahil edilecek ilişkili varlıklar (örn: <c>x => x.Category</c>).</param>
    /// <returns>Bulunan varlık veya <c>null</c>.</returns>
    public virtual async Task<TEntity?> GetByIdAsync(TId id, params Expression<Func<TEntity, object>>[] includes)
    {
        var query = Query().AsNoTracking();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        return await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
    }

    /// <summary>
    /// Bir filtreye uyan ilk varlığı, belirtilen ilişkili verileriyle (includes) birlikte getirir.
    /// Okuma performansı için <c>AsNoTracking()</c> metodu kullanılır.
    /// </summary>
    /// <param name="filter">Filtre koşulu.</param>
    /// <param name="includes">Sorguya dahil edilecek ilişkili varlıklar.</param>
    /// <returns>Filtreye uyan ilk varlık veya <c>null</c>.</returns>
    public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes)
    {
        var query = Query().AsNoTracking();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        return await query.FirstOrDefaultAsync(filter);
    }

    /// <summary>
    /// Bir filtreye uyan tüm varlıkları, belirtilen ilişkili verileriyle (includes) birlikte liste olarak getirir.
    /// Okuma performansı için <c>AsNoTracking()</c> metodu kullanılır.
    /// </summary>
    /// <param name="filter">Filtre koşulu (opsiyonel).</param>
    /// <param name="includes">Sorguya dahil edilecek ilişkili varlıklar.</param>
    /// <returns>Sonuçların listesi.</returns>
    public virtual async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes)
    {
        var query = Query().AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        query = includes.Aggregate(query, (current, include) => current.Include(include));

        return await query.ToListAsync();
    }

    /// <summary>
    /// ID'ye göre bir varlığı, belirtilen ilişkili verileriyle (includes) birlikte getirir.
    /// Okuma performansı için <c>AsNoTracking()</c> metodu varsayılan olarak kullanılır.
    /// <example>
    /// İlişkili verilerle birlikte tek bir kayıt getirme:
    /// <code>
    /// Ürünü, Kategori ve Marka bilgileriyle birlikte getir.
    /// var paginatedResult = await _productRepository.GetListAsync(
    ///     filter: p => p.Stock > 0 || p.IsActive,
    ///     orderBy: q => q.OrderByDescending(p => p.Price),
    ///     index: 0,
    ///     size: 10,
    ///     includes: p => p.Category
    /// );
    /// Dönen sonuçlar kullanılabilir:
    /// foreach(var product in paginatedResult.Items)
    /// {
    ///     Console.WriteLine(product.Name);
    /// }
    /// Console.WriteLine($"Toplam Sayfa: {paginatedResult.Pages}");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="filter">Filtre koşulu (opsiyonel).</param>
    /// <param name="orderBy">Sıralama koşulu (opsiyonel).</param>
    /// <param name="index">Sayfa indeksi (0'dan başlar).</param>
    /// <param name="size">Sayfa boyutu.</param>
    /// <param name="includes">Sorguya dahil edilecek ilişkili varlıklar.</param>
    /// <returns>Sayfalanmış veri yapısı (<c>IPaginate</c>).</returns>

    /// <summary>
    /// Bir filtreye uyan kayıt olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="filter">Filtre koşulu (opsiyonel).</param>
    /// <param name="cancellationToken">İşlem iptal token'ı.</param>
    /// <returns>Kayıt varsa <c>true</c>, yoksa <c>false</c>.</returns>
    public virtual async Task<IPaginate<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        int index = 0,
        int size = 20,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = Query().AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        query = includes.Aggregate(query, (current, include) => current.Include(include));

        if (orderBy != null)
            query = orderBy(query);

        return await query.ToPaginateAsync(index, size);
    }
    /// <summary>
    /// Bir filtreye uyan kayıt olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="filter">Filtre koşulu (opsiyonel).</param>
    /// <param name="cancellationToken">İşlem iptal token'ı.</param>
    /// <returns>Kayıt varsa <c>true</c>, yoksa <c>false</c>.</returns>
    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        return filter != null
            ? await Query().AnyAsync(filter, cancellationToken)
            : await Query().AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Bir filtreye uyan kayıtların toplam sayısını döndürür.
    /// <example>
    /// <code>
    /// // Stokta olmayan ürünlerin sayısı
    /// int count = await _productRepository.CountAsync(p => p.Stock == 0);
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="filter">Filtre koşulu (opsiyonel).</param>
    /// <param name="cancellationToken">İşlem iptal token'ı.</param>
    /// <returns>Toplam kayıt sayısı.</returns>
    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        return filter != null
            ? await Query().CountAsync(filter, cancellationToken)
            : await Query().CountAsync(cancellationToken);
    }
    #endregion
}