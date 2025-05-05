using Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Core.Persistence.Repositories;

public interface IRepository<TEntity, TId> where TEntity : Entity<TId>
{
    TEntity Add(TEntity entity);
    TEntity Update(TEntity entity);
    TEntity Delete(TEntity entity);
    List<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null, bool enableTracking = true, bool include = true);
    TEntity? Get(Expression<Func<TEntity, bool>> filter, bool enableTracking = true, bool include = true);

    bool Any(Expression<Func<TEntity, bool>>? filter = null, bool enableTracking = true);
    List<TEntity> GetList(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool enableTracking = true);
    IQueryable<TEntity> Query(); // Bu metot zaten vardı ve çok önemli
}