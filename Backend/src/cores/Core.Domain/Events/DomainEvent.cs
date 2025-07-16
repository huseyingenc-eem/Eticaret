namespace Core.Domain.Events;

/// <summary>
/// A base class for domain events, providing common properties.
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier for this event.
    /// </summary>
    public Guid EventId { get; protected set; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; protected set; }

    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}