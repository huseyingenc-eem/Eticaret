using Core.Application.Abstractions.Paging;
using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Specifications;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Core.Infrastructure.Persistence.Extensions;
namespace Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// IRepository arayüzü için Entity Framework Core'a özel temel implementasyon.
/// Tüm temel CRUD ve sorgulama işlemlerini jenerik olarak gerçekleştirir.
/// Bu sınıf, 'Core.Infrastructure' paketinizin temelini oluşturur ve yeniden kullanılabilir.
/// </summary>
public class EfRepositoryBase<TEntity, TId, TContext> : IRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
    where TContext : DbContext
{
    protected readonly TContext Context;

    public EfRepositoryBase(TContext context)
    {
        Context = context;
    }

    #region Yazma Operasyonları (Write Operations)

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await Context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
    public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }
        return Task.CompletedTask;
    }
    public Task DeleteAsync(TEntity entity, bool permanent = false, CancellationToken cancellationToken = default)
    {
        if (permanent || entity is not ISoftDeletable)
        {
            Context.Set<TEntity>().Remove(entity);
        }
        else
        {
            Context.Entry(entity).State = EntityState.Deleted;
        }
        return Task.CompletedTask;
    }

    public async Task DeleteByIdAsync(TId id, bool permanent = false, CancellationToken cancellationToken = default)
    {
        var entity = await Context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (entity != null)
        {
            await DeleteAsync(entity, permanent, cancellationToken);
        }
    }

    #endregion

    #region Okuma Operasyonları (Read Operations)

    public async Task<TEntity?> GetAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TEntity>> GetListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Verilen spesifikasyona uyan varlıkları sayfalanmış bir yapıda getirir.
    /// Bu metot, sizin sağladığınız ToPaginateAsync extension metodunu kullanır.
    /// </summary>
    /// <summary>
    /// Verilen spesifikasyona uyan varlıkları sayfalanmış bir yapıda getirir.
    /// Bu metot, projenizdeki mevcut ToPaginateAsync extension metodunu kullanır.
    /// </summary>
    public async Task<IPaginate<TEntity>> GetPaginatedListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        // 1. Filtre, Include ve Sıralama koşullarını uygula.
        IQueryable<TEntity> query = ApplySpecification(spec);

        // 2. Sayfalama için index ve size değerlerini spesifikasyondan al.
        int index = 0;
        int size = 20; // Varsayılan değerler
        if (spec.IsPagingEnabled)
        {
            // Take 0 olamaz, bölünme hatası verir.
            size = spec.Take > 0 ? spec.Take : size;
            index = spec.Skip / size;
        }

        // 3. Sorguyu, projenizdeki ToPaginateAsync extension metoduna gönder.
        // Bu metot arka planda Count, Skip, Take ve ToList işlemlerini kendisi yapacaktır.
        return await query.ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<TEntity>? spec = null, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).CountAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(ISpecification<TEntity>? spec = null, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).AnyAsync(cancellationToken);
    }

    #endregion

    #region Özel Yardımcı Metot (Private Helper)

    /// <summary>
    /// Tüm sorgu mantığının uygulandığı merkezi metottur.
    /// Verilen bir ISpecification nesnesini IQueryable'a dönüştürür.
    /// Bu, kod tekrarını önler ve sorgulama mantığını standartlaştırır.
    /// </summary>
    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity>? spec)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>().AsQueryable();

        // Her zaman soft-delete edilmiş olanları filtrele (eğer varlık bu davranışı destekliyorsa)
        if (typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)))
        {
            query = query.Where(e => ((ISoftDeletable)e).DeletedTime == null);
        }

        if (spec == null) return query;

        // Filtre (Where) koşulunu uygula
        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria);
        }

        // Eager Loading (Include) ifadelerini uygula
        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

        // Sıralama (OrderBy) koşullarını uygula
        if (spec.OrderBy != null)
        {
            query = query.OrderBy(spec.OrderBy);
        }
        else if (spec.OrderByDescending != null)
        {
            query = query.OrderByDescending(spec.OrderByDescending);
        }

        return query;
    }

    
    #endregion
}