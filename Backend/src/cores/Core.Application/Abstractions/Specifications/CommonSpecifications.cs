using Core.Domain.Entities;
using System.Linq.Expressions;

namespace Core.Application.Abstractions.Specifications;

/// <summary>
/// Yaygın kullanılan generic specification'lar için ortak sınıflar.
/// </summary>
public static class CommonSpecifications
{
    /// <summary>
    /// Belirtilen ID'ye sahip tek bir entity'yi getirmek için generic spesifikasyon.
    /// </summary>
    public class ByIdSpecification<TEntity, TId> : Specification<TEntity>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        public ByIdSpecification(TId id)
            : base(entity => entity.Id.Equals(id))
        {
        }
    }

    /// <summary>
    /// Belirtilen ID'ye sahip ve kullanıcı ID'si eşleşen entity'yi getirmek için generic spesifikasyon.
    /// </summary>
    public class ByIdAndUserIdSpecification<TEntity, TId> : Specification<TEntity>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        public ByIdAndUserIdSpecification(TId id, string userId)
            : base(entity => entity.Id.Equals(id) && GetUserId(entity) == userId)
        {
        }

        private static string GetUserId(TEntity entity)
        {
            // Reflection ile UserId property'sini bul
            var userIdProperty = typeof(TEntity).GetProperty("UserId");
            return userIdProperty?.GetValue(entity)?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// Aktif durumda olan entity'leri getirmek için generic spesifikasyon.
    /// </summary>
    public class ActiveEntitiesSpecification<TEntity> : Specification<TEntity>
        where TEntity : class
    {
        public ActiveEntitiesSpecification()
            : base(GetActiveFilter<TEntity>())
        {
        }

        private static Expression<Func<TEntity, bool>> GetActiveFilter<T>()
        {
            var parameter = Expression.Parameter(typeof(T), "entity");
            var property = typeof(T).GetProperty("IsActive");

            if (property != null && property.PropertyType == typeof(bool))
            {
                var propertyAccess = Expression.Property(parameter, property);
                var trueConstant = Expression.Constant(true);
                var equals = Expression.Equal(propertyAccess, trueConstant);

                return Expression.Lambda<Func<TEntity, bool>>(equals, parameter);
            }

            // IsActive property yoksa tüm kayıtları getir
            return entity => true;
        }
    }
}