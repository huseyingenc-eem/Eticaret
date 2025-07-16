namespace Core.Domain.Entities;

public interface IEntity<TId> where TId : notnull
{
    TId Id { get; init; }

    DateTime CreatedTime { get; }
    DateTime? UpdateTime { get; }
    DateTime? DeletedTime { get; }
}