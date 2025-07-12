using System;
using System.Linq.Expressions;

namespace Core.Application.Interfaces;

public interface IIncludeQuery<TEntity> where TEntity : class
{
    IIncludeQuery<TEntity> Include(Expression<Func<TEntity, object>> includeExpression);
    IIncludeQuery<TEntity> ThenInclude<TProperty>(Expression<Func<object, TProperty>> thenIncludeExpression);
}