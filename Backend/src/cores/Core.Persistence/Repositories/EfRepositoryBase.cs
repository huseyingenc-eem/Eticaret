using Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.Persistence.Repositories;

public class EfRepositoryBase<TEntity, TId, TContext> : IRepository<TEntity, TId>, IAsyncRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TContext : DbContext

{
    protected TContext _context { get; }

    public EfRepositoryBase(TContext context)
    {
        _context = context;
    }


    public TEntity Add(TEntity entity)
    {
        entity.CreatedTime = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Added;
        _context.SaveChanges();

        return entity;
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedTime = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Added;
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach(TEntity entity in entities)
        {
            entity.CreatedTime = DateTime.UtcNow;
            //_context.Entry(entity).State = EntityState.Added;
            //await _context.SaveChangesAsync(cancellationToken);
            //yield return entity;
            // en kötü yöntem.
        }
        await _context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entities;
    }

    public bool Any(Expression<Func<TEntity, bool>>? filter = null, bool enableTracking = true)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (enableTracking is false)
            query = query.AsNoTracking();

        if (filter is not null)
            return query.Any(filter);

        return query.Any();
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? filter = null, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (enableTracking is false)
            query = query.AsNoTracking();


        if (filter is not null)
            return await query.AnyAsync(filter, cancellationToken);

        return await query.AnyAsync(cancellationToken);
    }

    public TEntity Delete(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Deleted;
        _context.SaveChanges();

        return entity;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Entry(entity).State = EntityState.Deleted;
        await _context.SaveChangesAsync();

        return entity;
    }

    public TEntity? Get(Expression<Func<TEntity, bool>> filter, bool enableTracking = true, bool include = true)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (enableTracking is false)
            query = query.AsNoTracking();

        if (include is false)
            query = query.IgnoreAutoIncludes();

        return query.FirstOrDefault(filter);

    }
    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, bool enableTracking = true, bool include = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (enableTracking is false)
            query = query.AsNoTracking();

        if (include is false)
            query = query.IgnoreAutoIncludes();

        return await query.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public List<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null, bool enableTracking = true, bool include = true)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (filter is not null)
            query = query.Where(filter);


        if (enableTracking is false)
            query = query.AsNoTracking();

        if (include is false)
            query = query.IgnoreAutoIncludes();

        return query.ToList();
    }

    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, bool enableTracking = true, bool include = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (filter is not null)
            query = query.Where(filter);


        if (enableTracking is false)
            query = query.AsNoTracking();

        if (include is false)
            query = query.IgnoreAutoIncludes();

        return await query.ToListAsync(cancellationToken);
    }

    public TEntity Update(TEntity entity)
    {
        entity.UpdateTime = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        _context.SaveChanges();

        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.UpdateTime = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return entity;
    }
}
