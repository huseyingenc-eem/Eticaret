using Core.Persistence.Entities;
using Core.Persistence.Paging;
using Core.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Core.Infrastructure.Persistence.Repositories;

public class EfRepositoryBase<TEntity, TId, TContext> : IRepository<TEntity, TId>, IAsyncRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : IEquatable<TId>  
    where TContext : DbContext
{
    protected readonly TContext Context; 

    public EfRepositoryBase(TContext context)
    {
        Context = context;
    }
    #region Tanımlamalar
    /// <summary>
    /// Entity setine sorgulanabilir bir arayüz sağlar, yumuşak silinmiş kayıtları otomatik olarak filtreler.
    /// </summary>
    /// <returns>Yumuşak silinmiş kayıtları hariç tutan bir IQueryable<TEntity>.</returns> 
    public virtual IQueryable<TEntity> Query() => Context.Set<TEntity>().Where(e => e.DeletedTime == null);
    /// <summary>
    /// Entity setine herhangi bir varsayılan filtre olmadan (yumuşak silme dahil) sorgulanabilir bir arayüz sağlar.
    /// </summary>
    /// <returns>Ham DbSet'i temsil eden bir IQueryable<TEntity>.</returns> 
    public virtual IQueryable<TEntity> QueryAll() => Context.Set<TEntity>();

    #endregion

    #region Add Operations
    /// <summary>
    /// Bir entity'yi senkron olarak veritabanına eklemek için işaretler.
    /// </summary>
    /// <param name="entity"> Dışardan gelen Entity</param>
    /// <returns>İşaretlenen entity</returns>
    public virtual TEntity Add(TEntity entity)
    {
        entity.CreatedTime = DateTime.UtcNow;
        Context.Entry(entity).State = EntityState.Added;
        return entity;
    }

    /// <summary>
    /// Bir entity'yi asenkron olarak veritabanına eklemek için işaretler.
    /// </summary>
    /// <param name="entity">Dışardan gelen Entity</param>
    /// <returns>İşaretlenen entity</returns>
    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedTime = DateTime.UtcNow;
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken); 
        return entity;
    }

    /// <summary>
    /// Bir dizi entity'yi asenkron olarak veritabanına eklemek için işaretler.
    /// </summary>
    /// <param name="entities">Dışardan gelen Entitys</param>
    /// <returns>İşaretlenen entities</returns>
    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (TEntity entityInLoop in entities)
            entityInLoop.CreatedTime = DateTime.UtcNow;

        await Context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        return entities;
    }
    #endregion

    #region Update Operations
    /// <summary>
    /// Mevcut bir entity'yi senkron olarak güncellemek için işaretler.
    /// </summary>
    /// <param name="entity">Güncellenecek entity.</param>
    /// <returns>İşaretlenen entity.</returns>
    public virtual TEntity Update(TEntity entity)
    {
        entity.UpdateTime = DateTime.UtcNow;
        Context.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    /// <summary>
    /// Mevcut bir entity'yi asenkron olarak güncellemek için işaretler.
    /// </summary>
    /// <param name="entity">Güncellenecek entity.</param>
    /// <param name="cancellationToken">Görevin tamamlanmasını beklerken gözlemlenecek bir CancellationToken.</param>
    /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucu, işaretlenen entity'yi içerir.</returns>
    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        TEntity? existingEntity = await Context.Set<TEntity>().FindAsync(entity.Id);
        if (existingEntity != null)
        {
            Context.Entry(existingEntity).CurrentValues.SetValues(entity);
            existingEntity.UpdateTime = DateTime.UtcNow;
        }
        return entity;
    }

    /// <summary>
    /// Mevcut bir dizi entity'yi asenkron olarak güncellemek için işaretler.
    /// </summary>
    /// <param name="entities">Güncellenecek entity koleksiyonu</param>
    /// <param name="cancellationToken">Görevin tamamlanmasını beklerken gözlemlenecek bir CancellationToken.</param>
    /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucu, işaretlenen entity'leri içerir.</returns>
    public  virtual Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entityInLoop in entities)
        {
            entityInLoop.UpdateTime = DateTime.UtcNow;
        }
        Context.Set<TEntity>().UpdateRange(entities);
        return Task.FromResult(entities);
    }
    #endregion

    #region Delete Operations
    /// <summary>
    /// Bir entity'yi senkron olarak silmek için işaretler. Varsayılan olarak yumuşak silme yapar.
    /// </summary>
    /// <param name="entity">Silinecek entity.</param>
    /// <param name="permanent">True ise, fiziksel silme yapar. Aksi takdirde yumuşak silme yapar. Varsayılan: false.</param>
    /// <returns>İşaretlenen entity.</returns>
    public virtual TEntity Delete(TEntity entity, bool permanent = false)
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
        return entity;
    }

    /// <summary>
    /// Bir entity'yi asenkron olarak silmek için işaretler. Varsayılan olarak yumuşak silme yapar.
    /// </summary>
    /// <param name="entity">Silinecek entity</param>
    /// <param name="permanent"> True ise, fiziksel silme yapar. Aksi takdirde yumuşak silme yapar. Varsayılan: false</param>
    /// <param name="cancellationToken">Görevin tamamlanmasını beklerken gözlemlenecek bir CancellationToken.</param>
    /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucu, işaretlenen entity'yi içerir.</returns>
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
    /// Bir entity'yi ID'sine göre asenkron olarak silmek için işaretler. Varsayılan olarak yumuşak silme yapar.
    /// </summary>
    /// <param name="id">Silinecek entity'nin ID'si.</param>
    /// <param name="permanent">True ise, fiziksel silme yapar. Aksi takdirde yumuşak silme yapar. Varsayılan: false.</param>
    /// <param name="cancellationToken">Görevin tamamlanmasını beklerken gözlemlenecek bir CancellationToken.</param>
    /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucu, bulunursa işaretlenen entity'yi, aksi takdirde null içerir.</returns>
    public virtual async Task<TEntity?> DeleteByIdAsync(TId id, bool permanent = false, CancellationToken cancellationToken = default)
    {
        TEntity? entity = await (permanent ? QueryAll() : Query()).FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);

        if (entity == null)
            return null;

        await DeleteAsync(entity, permanent, cancellationToken); 
        return entity;
    }

    /// <summary>
    /// Bir entity'yi ID'sine göre senkron olarak silmek için işaretler. Varsayılan olarak yumuşak silme yapar.
    /// </summary>
    /// <param name="id">Silinecek entity'nin ID'si.</param>
    /// <param name="permanent">True ise, fiziksel silme yapar. Aksi takdirde yumuşak silme yapar. Varsayılan: false.</param>
    /// <returns> Bulunursa işaretlenen entity'yi, aksi takdirde null içerir.</returns>
    public virtual TEntity? DeleteById(TId id, bool permanent = false)
    {
        TEntity? entity = (permanent ? QueryAll() : Query()).FirstOrDefault(e => e.Id.Equals(id));

        if (entity == null)
            return null;

        return Delete(entity, permanent);
    }

    /// <summary>
    /// Bir dizi entity'yi asenkron olarak silmek için işaretler. Varsayılan olarak yumuşak silme yapar.
    /// </summary>
    /// <param name="entities">Silinecek entity koleksiyonu.</param>
    /// <param name="permanent">True ise, fiziksel silme yapar. Aksi takdirde yumuşak silme yapar. Varsayılan: false.</param>
    /// <param name="cancellationToken">Görevin tamamlanmasını beklerken gözlemlenecek bir CancellationToken.</param>
    /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucu, işaretlenen entity'leri içerir.</returns>
    public virtual Task<IEnumerable<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entities, bool permanent = false, CancellationToken cancellationToken = default)
    {
        if (!permanent)
        {
            foreach (var entityInLoop in entities)
            {
                entityInLoop.DeletedTime = DateTime.UtcNow;
                entityInLoop.UpdateTime = DateTime.UtcNow;
                Context.Entry(entityInLoop).State = EntityState.Modified;
            }
        }
        else
        {
            Context.Set<TEntity>().RemoveRange(entities);
        }
        return Task.FromResult(entities);
    }
    #endregion

    #region Read Operations
    /// <summary>
    /// Bir koşulu sağlayan herhangi bir entity olup olmadığını kontrol eder. Varsayılan olarak yumuşak silme filtresi kullanır.
    /// </summary>
    /// <param name="filter">Entity'leri filtrelemek için bir ifade. Null olabilir.</param>
    /// <param name="enableTracking">Değişiklik izlemeyi etkinleştirmek için bir boolean. Varsayılan: true.</param>
    /// <returns>Koşulu sağlayan herhangi bir entity varsa true; aksi takdirde false.</returns> 
    public virtual bool Any(Expression<Func<TEntity, bool>>? filter = null, bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (filter != null) return queryable.Any(filter);
        return queryable.Any();
    }

    /// <summary>
    /// Bir koşulu sağlayan herhangi bir entity olup olmadığını asenkron olarak kontrol eder. Varsayılan olarak yumuşak silme filtresi kullanır.
    /// </summary>
    /// <param name="filter">Entity'leri filtrelemek için bir ifade. Null olabilir.</param>
    /// <param name="enableTracking">Değişiklik izlemeyi etkinleştirmek için bir boolean. Varsayılan: true.</param>
    /// <param name="cancellationToken">Görevin tamamlanmasını beklerken gözlemlenecek bir CancellationToken.</param>
    /// <returns> Asenkron işlemi temsil eden bir görev. Görev sonucu, koşulu sağlayan herhangi bir entity varsa true; aksi takdirde false içerir.</returns>
    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? filter = null, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (filter != null) return await queryable.AnyAsync(filter, cancellationToken);
        return await queryable.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Bir entity'yi ID'sine göre senkron olarak alır. Varsayılan olarak yumuşak silme filtresi kullanır.
    /// </summary>
    /// <param name="id">Alınacak entity'nin ID'si.</param>
    /// <param name="include"> İlişkili entity'leri dahil etmek için bir fonksiyon.</param>
    /// <param name="enableTracking">Değişiklik izlemeyi etkinleştirmek için bir boolean. Varsayılan: true.</param>
    /// <returns> Bulunursa (ve yumuşak silinmemişse) entity; aksi takdirde null.</returns>
    public virtual TEntity? Get(TId id,
                                Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (include != null) queryable = include(queryable);
        return queryable.FirstOrDefault(e => e.Id.Equals(id));
    }

    /// <summary>
    /// /Bir entity'yi ID'sine göre asenkron olarak alır. Varsayılan olarak yumuşak silme filtresi kullanır.
    /// </summary>
    /// <param name="id">Alınacak entity'nin ID'si.</param>
    /// <param name="include">İlişkili entity'leri dahil etmek için bir fonksiyon.</param>
    /// <param name="enableTracking">Değişiklik izlemeyi etkinleştirmek için bir boolean. Varsayılan: true.</param>
    /// <param name="cancellationToken">Görevin tamamlanmasını beklerken gözlemlenecek bir CancellationToken.</param>
    /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucu, bulunursa (ve yumuşak silinmemişse) entity; aksi takdirde null içerir.</returns>
    public virtual async Task<TEntity?> GetAsync(TId id,
                                                 Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                                 bool enableTracking = true,
                                                 CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (include != null) queryable = include(queryable);
        return await queryable.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    /// <summary>
    /// /Bir koşula göre tek bir entity'yi senkron olarak alır. Varsayılan olarak yumuşak silme filtresi kullanır.
    /// </summary>
    /// <param name="filter">Entity'leri filtrelemek için bir ifade.</param>
    /// <param name="include">İlişkili entity'leri dahil etmek için bir fonksiyon.</param>
    /// <param name="enableTracking">Değişiklik izlemeyi etkinleştirmek için bir boolean. Varsayılan: true.</param>
    /// <returns>Bulunursa (ve yumuşak silinmemişse) entity; aksi takdirde null.</returns>
    public virtual TEntity? Get(Expression<Func<TEntity, bool>> filter,
                                Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (include != null) queryable = include(queryable);
        return queryable.FirstOrDefault(filter);
    }

    /// <summary>
    /// Bir koşula göre tek bir entity'yi asenkron olarak alır. Varsayılan olarak yumuşak silme filtresi kullanır.
    /// </summary>
    /// <param name="filter">Entity'leri filtrelemek için bir ifade.</param>
    /// <param name="include"> İlişkili entity'leri dahil etmek için bir fonksiyon.</param>
    /// <param name="enableTracking">Değişiklik izlemeyi etkinleştirmek için bir boolean. Varsayılan: true.</param>
    /// <param name="cancellationToken">Görevin tamamlanmasını beklerken gözlemlenecek bir CancellationToken.</param>
    /// <returns>Görev sonucu, bulunursa (ve yumuşak silinmemişse) entity; aksi takdirde null içerir.</returns>
    public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter,
                                               Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                               bool enableTracking = true,
                                               CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (include != null) queryable = include(queryable);
        return await queryable.FirstOrDefaultAsync(filter, cancellationToken);
    }

    /// <summary>
    /// Bir filtreyi sağlayan tüm entity'lerin listesini senkron olarak alır. Varsayılan olarak yumuşak silme filtresi kullanır.
    /// </summary>
    /// <param name="filter">Entity'leri filtrelemek için bir ifade. Tüm (yumuşak silinmemiş) entity'leri almak için null olabilir.</param>
    /// <param name="include">İlişkili entity'leri dahil etmek için bir fonksiyon.</param>
    /// <param name="enableTracking">Değişiklik izlemeyi etkinleştirmek için bir boolean. Varsayılan: true.</param>
    /// <returns>Entity listesi.</returns> 
    public virtual List<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null,
                                        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                        bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query();
        if (filter != null) queryable = queryable.Where(filter);
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (include != null) queryable = include(queryable);
        return queryable.ToList();
    }

    /// <summary>
    /// Bir filtreyi sağlayan tüm entity'lerin listesini asenkron olarak alır. Varsayılan olarak yumuşak silme filtresi kullanır.
    /// </summary>
    /// <param name="filter">Entity'leri filtrelemek için bir ifade. Null olabilir.</param>
    /// <param name="include">İlişkili entity'leri dahil etmek için bir fonksiyon.</param>
    /// <param name="enableTracking">Değişiklik izlemeyi etkinleştirmek için bir boolean. Varsayılan: true.</param>
    /// <param name="cancellationToken">Görevin tamamlanmasını beklerken gözlemlenecek bir CancellationToken.</param>
    /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucu, entity listesini içerir.</returns>
    public virtual async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null,
                                                       Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                                       bool enableTracking = true,
                                                       CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (filter != null) queryable = queryable.Where(filter);
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (include != null) queryable = include(queryable);
        return await queryable.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Sayfalanmış bir entity listesini senkron olarak alır. Varsayılan olarak yumuşak silme filtresi kullanır.
    /// </summary>
    /// <param name="filter">Entity'leri filtrelemek için bir ifade. Null olabilir.</param>
    /// <param name="orderBy">Entity'leri sıralamak için bir fonksiyon. Null olabilir.</param>
    /// <param name="index">Sıfır tabanlı sayfa indeksi. Varsayılan: 0.</param>
    /// <param name="size">Sayfa boyutu. Varsayılan: 10.</param>
    /// <param name="include">İlişkili entity'leri dahil etmek için bir fonksiyon.</param>
    /// <param name="enableTracking">Değişiklik izlemeyi etkinleştirmek için bir boolean. Varsayılan: true.</param>
    /// <returns>Sayfalanmış entity listesini temsil eden bir IPaginate<TEntity>.</returns>
    public virtual IPaginate<TEntity> GetList(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        int index = 0,
        int size = 20,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (include != null) queryable = include(queryable);
        if (filter != null) queryable = queryable.Where(filter);
        if (orderBy != null)
            return orderBy(queryable).ToPaginate(index, size);

        return queryable.ToPaginate(index, size);
    }

    /// <summary>
    /// Sayfalanmış bir entity listesini asenkron olarak alır. Varsayılan olarak yumuşak silme filtresi kullanır.
    /// </summary>
    /// <param name="filter">Entity'leri filtrelemek için bir ifade. Null olabilir.</param>
    /// <param name="orderBy">Entity'leri sıralamak için bir fonksiyon. Null olabilir.</param>
    /// <param name="index">Sıfır tabanlı sayfa indeksi. Varsayılan: 0.</param>
    /// <param name="size">Sayfa boyutu. Varsayılan: 20 (kodunuzda 20 idi, burada da 20 olarak bırakıldı).</param>
    /// <param name="include">İlişkili entity'leri dahil etmek için bir fonksiyon.</param>
    /// <param name="enableTracking">Değişiklik izlemeyi etkinleştirmek için bir boolean. Varsayılan: true.</param>
    /// <param name="cancellationToken">Görevin tamamlanmasını beklerken gözlemlenecek bir CancellationToken.</param>
    /// <returns>sayfalanmış entity listesini temsil eden bir IPaginate<TEntity> içerir.</returns>
    public virtual async Task<IPaginate<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        int index = 0,
        int size = 20,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query(); 
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (include != null) queryable = include(queryable);
        if (filter != null) queryable = queryable.Where(filter);
        if (orderBy != null)
            return await orderBy(queryable).ToPaginateAsync(index, size, cancellationToken: cancellationToken);

        return await queryable.ToPaginateAsync(index, size, cancellationToken: cancellationToken);
    }
    #endregion
}
