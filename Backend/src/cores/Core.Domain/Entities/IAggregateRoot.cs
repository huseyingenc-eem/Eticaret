namespace Core.Domain.Entities;

/// <summary>
/// Represents an aggregate root in the domain.
/// An aggregate root is a specific type of entity that acts as a transactional boundary
/// for a cluster of related objects.
/// This is a marker interface.
/// </summary>
public interface IAggregateRoot { }