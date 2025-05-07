using Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Core.Persistence.Repositories;

public class EfRepositoryBase<TEntity, TId, TContext> : IRepository<TEntity, TId>, IAsyncRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TContext : DbContext
{
    protected TContext Context { get; }

    public EfRepositoryBase(TContext context)
    {
        Context = context;
    }


    public TEntity Add(TEntity entity)
    {
        entity.CreatedTime = DateTime.UtcNow;
        Context.Entry(entity).State = EntityState.Added;
        return entity;
    }

    public Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedTime = DateTime.UtcNow;
        Context.Entry(entity).State = EntityState.Added;
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (TEntity entity in entities)
        {
            entity.CreatedTime = DateTime.UtcNow;
        }
        Context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        return Task.FromResult(entities);
    }

    public TEntity Update(TEntity entity)
    {
        entity.UpdateTime = DateTime.UtcNow;
        Context.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.UpdateTime = DateTime.UtcNow;
        Context.Entry(entity).State = EntityState.Modified;
        return Task.FromResult(entity);
    }
    public Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            entity.UpdateTime = DateTime.UtcNow;
        }
        Context.Set<TEntity>().UpdateRange(entities);
        return Task.FromResult(entities);
    }


    public TEntity Delete(TEntity entity)
    {
        Context.Entry(entity).State = EntityState.Deleted;
        return entity;
    }

    public Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Entry(entity).State = EntityState.Deleted;
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().RemoveRange(entities);
        return Task.FromResult(entities);
    }


    // --- Read Operations (DEĞİŞİKLİK YOK) ---

    public bool Any(Expression<Func<TEntity, bool>>? filter = null, bool enableTracking = true)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();
        if (!enableTracking) query = query.AsNoTracking();
        if (filter != null) return query.Any(filter);
        return query.Any();
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? filter = null, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();
        if (!enableTracking) query = query.AsNoTracking();
        if (filter != null) return await query.AnyAsync(filter, cancellationToken);
        return await query.AnyAsync(cancellationToken);
    }

    public TEntity? Get(Expression<Func<TEntity, bool>> filter, bool enableTracking = true, bool include = true)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();
        if (!enableTracking) query = query.AsNoTracking();
        // Not: IgnoreAutoIncludes deprecated olabilir, model konfigürasyonunda yönetmek daha iyi.
        if (!include) query = query.IgnoreAutoIncludes();
        return query.FirstOrDefault(filter);
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, bool enableTracking = true, bool include = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();
        if (!enableTracking) query = query.AsNoTracking();
        if (!include) query = query.IgnoreAutoIncludes();
        return await query.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public List<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null, bool enableTracking = true, bool include = true)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();
        if (filter != null) query = query.Where(filter);
        if (!enableTracking) query = query.AsNoTracking();
        if (!include) query = query.IgnoreAutoIncludes();
        return query.ToList();
    }

    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, bool enableTracking = true, bool include = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();
        if (filter != null) query = query.Where(filter);
        if (!enableTracking) query = query.AsNoTracking();
        if (!include) query = query.IgnoreAutoIncludes();
        return await query.ToListAsync(cancellationToken);
    }

    public IQueryable<TEntity> Query() => Context.Set<TEntity>();

    public virtual List<TEntity> GetList(
       Expression<Func<TEntity, bool>>? filter = null,
       Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
       
       bool include = true,
       bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (!include) queryable = queryable.IgnoreAutoIncludes();

        if (filter != null) queryable = queryable.Where(filter);
        if (orderBy != null) return orderBy(queryable).ToList();
        return queryable.ToList();
    }

    public async Task<List<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool include = true,
        bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (!include) queryable = queryable.IgnoreAutoIncludes();

        if (filter != null) queryable = queryable.Where(filter);
        if (orderBy != null) return await orderBy(queryable).ToListAsync(cancellationToken);
        return await queryable.ToListAsync(cancellationToken);
    }
}
