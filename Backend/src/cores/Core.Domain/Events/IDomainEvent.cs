namespace Core.Domain.Events;

/// <summary>
/// Represents a domain event, which is something that has happened in the domain.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// The unique identifier for this specific event instance.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// The date and time when the event occurred.
    /// </summary>
    DateTime OccurredOn { get; }
}