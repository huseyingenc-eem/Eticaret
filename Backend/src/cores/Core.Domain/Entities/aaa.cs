namespace Core.Domain.Entities;

public abstract class BaseEntity<TId> : IEntity<TId>
{
    public TId Id { get; set; } = default(TId)!;
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeletedTime { get; set; }
}