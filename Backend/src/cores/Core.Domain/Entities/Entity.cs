namespace Core.Domain.Entities;

public abstract class Entity<TId> : IEntity<TId>
{
    public TId Id { get; set; } = default(TId)!;
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeletedTime { get; set; }


    public bool Equals(TId other)
    {
        if (other is null) return false;
        return Id.Equals(other);
    }

    public override bool Equals(object obj)
    {
        return obj is Entity<TId> entity && Id.Equals(entity.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}