using Core.Domain.Enums;
using Core.Domain.Events;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities;

/// <summary>
/// Tüm domain varlıkları için ortak işlevsellik sağlayan temel bir sınıfı temsil eder.
/// Bu sınıf kimlik, eşitlik karşılaştırması, soft-delete ve domain event yönetimi gibi özellikleri barındırır.
/// </summary>
/// <typeparam name="TId">Varlığın benzersiz kimliğinin türü (örn: int, Guid).</typeparam>
public abstract class Entity<TId> : IEntity<TId>, ISoftDeletable, IEquatable<Entity<TId>> where TId : notnull
{
    #region Constructors

    /// <summary>
    /// Parametresiz kurucu metot. Varlık oluşturulduğunda CreatedTime'ı otomatik olarak UTC zaman damgası ile ayarlar.
    /// Bu constructor, Entity Framework Core gibi ORM'ler tarafından gereklidir.
    /// </summary>
    protected Entity()
    {
        CreatedTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Belirtilen kimlik ile yeni bir varlık örneği başlatır.
    /// </summary>
    /// <param name="id">Varlığın benzersiz kimliği.</param>
    protected Entity(TId id) : this()
    {
        Id = id;
    }

    #endregion

    #region Properties

    public TId Id { get; protected set; }

    /// <summary>Varlığın oluşturulma zamanı (UTC).</summary>
    public DateTime CreatedTime { get; protected set; }

    /// <summary>Varlığın son güncellenme zamanı (UTC). Hiç güncellenmemişse null olabilir.</summary>
    public DateTime? UpdateTime { get; protected set; }

    /// <summary>Varlığın silinme zamanı (UTC). Varlık silinmemişse (soft-delete) null'dır.</summary>
    public DateTime? DeletedTime { get; protected set; }

    /// <summary>Varlığın mevcut durumu (örn: Aktif, Pasif, Silinmiş).</summary>
    public EntityStatus Status { get; protected set; }

    #endregion

    #region Domain Events

    [NotMapped] // Bu alanın veritabanı şemasına dahil edilmemesini sağlar.
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Bu varlık tarafından tetiklenen domain olaylarının salt okunur bir koleksiyonunu alır.
    /// Bu olaylar, varlığın yaşam döngüsündeki önemli değişiklikleri temsil eder.
    /// </summary>
    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Varlığın iç olay listesine bir domain olayı ekler.
    /// </summary>
    /// <param name="domainEvent">Eklenecek domain olayı.</param>
    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Varlıktaki tüm domain olaylarını temizler.
    /// Bu metot, olaylar dağıtılıp işlendikten sonra çağrılmalıdır.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    #endregion

    #region Methods

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
    /// Varlığın geçici (transient) olup olmadığını, yani henüz bir kimliğe sahip olup olmadığını kontrol eder.
    /// </summary>
    /// <returns>Kimlik varsayılan değere sahipse true, aksi takdirde false.</returns>
    private bool IsTransient() => Id.Equals(default(TId));

    #endregion

    #region Equality & Operators

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

        // Geçici (henüz veritabanına kaydedilmemiş) varlıklar veya farklı tiplerdeki varlıklar Id'leri üzerinden karşılaştırılamaz.
        if (IsTransient() || other.IsTransient() || GetType() != other.GetType())
        {
            return false;
        }

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Varsayılan karma işlevi olarak hizmet eder. Karma kod, varlığın kimliğine dayanır.
    /// </summary>
    /// <returns>Mevcut varlık için bir karma kod.</returns>
    public override int GetHashCode()
    {
        // Varlık transient değilse kimliğinin hash kodunu kullan.
        return IsTransient() ? base.GetHashCode() : Id.GetHashCode() * 31;
    }

    /// <summary>
    /// İki varlığın eşit olup olmadığını karşılaştırır.
    /// </summary>
    /// <param name="left">Sol taraftaki varlık.</param>
    /// <param name="right">Sağ taraftaki varlık.</param>
    /// <returns>Varlıklar eşitse true; aksi takdirde false.</returns>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        // Eğer her ikisi de null ise veya sadece soldaki null ise, sol'un Equals metodu doğru sonucu verir.
        return left is null ? right is null : left.Equals(right);
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

    #endregion
}