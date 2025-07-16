using Core.Domain.Enums;
using Core.Domain.Events;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities;

/// <summary>
/// Tüm domain varlıkları için ortak işlevsellik sağlayan temel bir sınıfı temsil eder.
/// Bu sınıf kimlik, eşitlik karşılaştırması ve domain event yönetimi gibi özellikleri barındırır.
/// </summary>
/// <typeparam name="TId">Varlığın benzersiz kimliğinin türü (örn: int, Guid).</typeparam>
public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>> where TId : notnull
{
    /// <summary>
    /// Varlığın benzersiz kimliğini alır.
    /// 'setter' metodunun 'protected' olması, kimliğin varlık sınırı dışından değiştirilemez olmasını sağlar.
    /// </summary>
    public TId Id { get; init; }

    /// <summary>
    /// Varlığın oluşturulduğu zaman damgasını alır.
    /// </summary>
    public DateTime CreatedTime { get; protected set; }

    /// <summary>
    /// Varlığın son güncellendiği zaman damgasını alır. Varlık hiç güncellenmemişse null'dır.
    /// </summary>
    public DateTime? UpdateTime { get; protected set; }

    /// <summary>
    /// Varlığın silindi olarak işaretlendiği zaman damgasını alır. Silinmemişse null'dır.
    /// </summary>
    public DateTime? DeletedTime { get; protected set; }

    /// <summary>
    /// Varlığın mevcut durumunu alır.
    /// </summary>
    public EntityStatus Status { get; protected set; }

    [NotMapped]
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Bu varlık tarafından tetiklenen domain event'lerinin salt okunur bir koleksiyonunu alır.
    /// Bu event'ler, varlığın yaşam döngüsündeki önemli değişiklikleri temsil eder.
    /// </summary>
    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// <see cref="Entity{TId}"/> sınıfının yeni bir örneğini belirli bir kimlikle başlatır.
    /// </summary>
    /// <param name="id">Varlık için benzersiz kimlik.</param>
    protected Entity(TId id)
    {
        Id = id;
    }

    /// <summary>
    /// <see cref="Entity{TId}"/> sınıfının yeni bir örneğini başlatır.
    /// Bu parametresiz kurucu metot, Entity Framework Core gibi ORM'ler için gereklidir.
    /// </summary>
    protected Entity() { }

    /// <summary>
    /// Varlığın oluşturulma zaman damgasını ayarlar.
    /// </summary>
    /// <param name="createdTime">Varlığın oluşturulduğu Eşgüdümlü Evrensel Zaman (UTC).</param>
    public virtual void SetCreatedTime(DateTime createdTime) => CreatedTime = createdTime;

    /// <summary>
    /// Varlığın güncellenme zaman damgasını ayarlar.
    /// </summary>
    /// <param name="updatedTime">Varlığın güncellendiği Eşgüdümlü Evrensel Zaman (UTC).</param>
    public virtual void SetUpdatedTime(DateTime updatedTime) => UpdateTime = updatedTime;

    /// <summary>
    /// Varlığın silinme zaman damgasını ayarlar.
    /// </summary>
    /// <param name="deletedTime">Varlığın silindiği Eşgüdümlü Evrensel Zaman (UTC).</param>
    public virtual void SetDeletedTime(DateTime deletedTime) => DeletedTime = deletedTime;

    /// <summary>
    /// Varlığın iç olay listesine bir domain event ekler.
    /// </summary>
    /// <param name="domainEvent">Eklenecek domain event.</param>
    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Varlıktaki tüm domain event'leri temizler.
    /// Bu metot, olaylar dağıtılıp işlendikten sonra çağrılmalıdır.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>
    /// Belirtilen nesnenin mevcut varlığa eşit olup olmadığını belirler.
    /// </summary>
    /// <param name="obj">Mevcut varlıkla karşılaştırılacak nesne.</param>
    /// <returns>Belirtilen nesne mevcut varlığa eşitse true; aksi takdirde false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity && Equals(entity);
    }

    /// <summary>
    /// Belirtilen varlığın, kimliklerine göre mevcut varlığa eşit olup olmadığını belirler.
    /// </summary>
    /// <param name="other">Mevcut varlıkla karşılaştırılacak diğer varlık.</param>
    /// <returns>Varlıklar aynı kimliğe sahipse true; aksi takdirde false.</returns>
    public virtual bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (IsTransient() || other.IsTransient() || GetType() != other.GetType())
        {
            return false;
        }
        return Id.Equals(other.Id);
    }

    private bool IsTransient() => Id.Equals(default(TId));

    /// <summary>
    /// Varsayılan karma işlevi olarak hizmet eder. Karma kod, varlığın kimliğine dayanır.
    /// </summary>
    /// <returns>Mevcut varlık için bir karma kod.</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode() * 31;
    }

    /// <summary>
    /// İki varlığın eşit olup olmadığını karşılaştırır.
    /// </summary>
    /// <param name="left">Sol taraftaki varlık.</param>
    /// <param name="right">Sağ taraftaki varlık.</param>
    /// <returns>Varlıklar eşitse true; aksi takdirde false.</returns>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null)
        {
            return right is null;
        }
        return left.Equals(right);
    }

    /// <summary>
    /// İki varlığın eşit olmadığını karşılaştırır.
    /// </summary>
    /// <param name="left">Sol taraftaki varlık.</param>
    /// <param name="right">Sağ taraftaki varlık.</param>
    /// <returns>Varlıklar eşit değilse true; aksi takdirde false.</returns>
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }
}