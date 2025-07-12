namespace Core.Domain.Entities;

public interface IEntity<TId>
{
    TId Id { get; set; }
    DateTime CreatedTime { get; set; }
    DateTime? UpdateTime { get; set; }
    DateTime? DeletedTime { get; set; }
}