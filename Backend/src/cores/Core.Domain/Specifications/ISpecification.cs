using System.Linq.Expressions;

namespace Core.Domain.Specifications;

/// <summary>
/// Belirli bir varlık türü için sorgu kriterlerini tanımlayan spesifikasyon arayüzü.
/// </summary>
/// <typeparam name="T">Spesifikasyonun uygulanacağı varlık türü.</typeparam>
public  interface ISpecification<T>
{
    /// <summary>
    /// Sorgu için filtreleme kriteri ('Where' koşulu).
    /// </summary>
    Expression<Func<T, bool>> Criteria { get; }

    /// <summary>
    /// Sorguya dahil edilecek ilişkili varlıkların listesi (eager loading).
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Sonuçları artan düzende sıralamak için kullanılacak ifade.
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Sonuçları azalan düzende sıralamak için kullanılacak ifade.
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }
}