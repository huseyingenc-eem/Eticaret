using Core.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.Infrastructure.Persistence.Repositories;

public class EfIncludeQuery<TEntity> : IIncludeQuery<TEntity> where TEntity : class
{
    private IQueryable<TEntity> _queryable;

    public EfIncludeQuery(IQueryable<TEntity> queryable)
    {
        _queryable = queryable;
    }

    public IIncludeQuery<TEntity> Include(Expression<Func<TEntity, object>> includeExpression)
    {
        _queryable = _queryable.Include(includeExpression);
        return this;
    }

    public IIncludeQuery<TEntity> ThenInclude<TProperty>(Expression<Func<object, TProperty>> thenIncludeExpression)
    {
        // Note: ThenInclude implementation would need more complex handling
        // This is a simplified version
        return this;
    }

    public IQueryable<TEntity> GetQueryable() => _queryable;
}