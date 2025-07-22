using System.Linq.Expressions;

namespace Core.Shared.Helpers;

/// <summary>
/// LINQ Expression ağaçları üzerinde işlem yapmak için yardımcı metotlar sağlar.
/// </summary>
public static class ExpressionHelper
{
    /// <summary>
    /// İki LINQ Expression'ını bir "AND" (ve) mantıksal operatörü ile birleştirir.
    /// </summary>
    /// <typeparam name="T">Expression'ın hedef türü.</typeparam>
    /// <param name="left">Sol taraftaki ifade.</param>
    /// <param name="right">Sağ taraftaki ifade.</param>
    /// <returns>İki ifadenin "AND" ile birleştirilmiş hali olan yeni bir Expression.</returns>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], parameter);
        var leftBody = leftVisitor.Visit(left.Body);

        var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], parameter);
        var rightBody = rightVisitor.Visit(right.Body);

        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(leftBody, rightBody), parameter);
    }

    /// <summary>
    /// İki LINQ Expression'ını bir "OR" (veya) mantıksal operatörü ile birleştirir.
    /// </summary>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], parameter);
        var leftBody = leftVisitor.Visit(left.Body);

        var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], parameter);
        var rightBody = rightVisitor.Visit(right.Body);

        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(leftBody, rightBody), parameter);
    }

    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression? node)
        {
            return node == _oldValue ? _newValue : base.Visit(node)!;
        }
    }
}