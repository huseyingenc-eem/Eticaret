using Core.Domain.Events;

namespace Core.Domain.Entities;

/// <summary>
/// Bir domain varlığının temel sözleşmesini tanımlar.
/// </summary>
/// <typeparam name="TId">Varlığın benzersiz kimliğinin türü.</typeparam>
public interface IEntity<out TId> where TId : notnull
{
    /// <summary>
    /// Varlığın benzersiz kimliğini alır.
    /// 'out' anahtar kelimesi, TId'nin kovaryant olmasını sağlar.
    /// </summary>
    TId Id { get; }

    DateTime CreatedTime { get; }
    DateTime? UpdateTime { get; }
    DateTime? DeletedTime { get; }

    /// <summary>
    /// Bu varlıkta meydana gelen domain olaylarının bir koleksiyonunu alır.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Varlıktaki tüm domain olaylarını temizler.
    /// </summary>
    void ClearDomainEvents();
}