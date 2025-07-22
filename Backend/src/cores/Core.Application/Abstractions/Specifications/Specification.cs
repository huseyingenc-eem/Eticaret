using System.Linq.Expressions;

namespace Core.Application.Abstractions.Specifications;

/// <summary>
/// 'ISpecification' arayüzü için ortak işlevsellik sağlayan soyut bir temel sınıftır.
/// Bu sınıf, yeni spesifikasyonlar oluştururken tekrar eden kodları (boilerplate) ortadan kaldırmak
/// ve sorgu yapılandırmasını daha basit hale getirmek için tasarlanmıştır.
/// Kendi spesifikasyonlarınızı (örn: ActiveUsersSpecification) bu sınıftan türeterek oluşturursunuz.
/// </summary>
/// <typeparam name="T">Spesifikasyonun hedeflendiği varlık türü.</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    /// <inheritdoc/>
    public Expression<Func<T, bool>> Criteria { get; }

    /// <inheritdoc/>
    public List<Expression<Func<T, object>>> Includes { get; } = new();

    /// <inheritdoc/>
    public Expression<Func<T, object>>? OrderBy { get; private set; }

    /// <inheritdoc/>
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    /// <summary>
    /// Specification sınıfının yeni bir örneğini, zorunlu bir filtre kriteri ile başlatır.
    /// Her spesifikasyonun mutlaka bir 'Where' koşulu olmalıdır, bu nedenle bu kriter kurucu metot (constructor)
    /// aracılığıyla alınır.
    /// </summary>
    /// <param name="criteria">Sorgu için kullanılacak filtreleme ifadesi.</param>
    protected Specification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// (Protected) Sorguya 'eager loading' için bir 'Include' ifadesi ekler.
    /// Bu metot, türetilmiş spesifikasyon sınıflarının kurucuları içinde çağrılarak,
    /// sorguya hangi ilişkili verilerin dahil edileceğini kolayca belirtmek için kullanılır.
    /// </summary>
    /// <param name="includeExpression">Dahil edilecek ilişkili varlık için LINQ ifadesi (örn: x => x.Orders).</param>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// (Protected) Sorgu için artan sıralama kriterini belirler.
    /// Bu metot, türetilmiş spesifikasyon sınıflarının kurucuları içinde çağrılarak,
    /// sonuçların hangi alana göre artan sırada sıralanacağını belirtir.
    /// </summary>
    /// <param name="orderByExpression">Sıralama için kullanılacak alanın ifadesi (örn: x => x.Name).</param>
    protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// (Protected) Sorgu için azalan sıralama kriterini belirler.
    /// Bu metot, türetilmiş spesifikasyon sınıflarının kurucuları içinde çağrılarak,
    /// sonuçların hangi alana göre azalan sırada sıralanacağını belirtir.
    /// </summary>
    /// <param name="orderByDescendingExpression">Sıralama için kullanılacak alanın ifadesi (örn: x => x.CreatedAt).</param>
    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    /// <summary>
    /// Sorgu için sayfalama uygular. Bu metot çağrıldığında sayfalama aktif hale gelir.
    /// </summary>
    /// <param name="skip">Atlanacak kayıt sayısı.</param>
    /// <param name="take">Alınacak kayıt sayısı.</param>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}