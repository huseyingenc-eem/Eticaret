using System.Linq.Expressions;

namespace Core.Domain.Specifications;

/// <summary>
/// Ortak işlevsellik sağlayan, spesifikasyonlar için temel sınıf.
/// </summary>
/// <typeparam name="T">Varlık türü.</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    /// <summary>
    /// Verilen filtre kriteriyle <see cref="Specification{T}"/> sınıfının yeni bir örneğini başlatır.
    /// </summary>
    /// <param name="criteria">Filtreleme kriteri.</param>
    protected Specification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Sorgu sonucuna dahil edilecek ilişkili bir varlık ekler.
    /// </summary>
    /// <param name="includeExpression">Dahil edilecek varlık için ifade.</param>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// Sorgu için artan sıralama kriterini belirler.
    /// </summary>
    protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// Sorgu için azalan sıralama kriterini belirler.
    /// </summary>
    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }
}
