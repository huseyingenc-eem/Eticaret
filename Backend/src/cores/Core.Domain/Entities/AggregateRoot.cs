namespace Core.Domain.Entities;

/// <summary>
/// Base class for aggregate roots. It inherits from Entity and implements IAggregateRoot.
/// It provides the core functionalities of an entity, including domain event management,
/// and marks the class as an aggregate root.
/// </summary>
/// <typeparam name="TId">The type of the entity's unique identifier.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot{TId}"/> class with a specific identifier.
    /// </summary>
    /// <param name="id">The unique identifier for the aggregate root.</param>
    protected AggregateRoot(TId id) : base(id) { }

    /// <summary>
    /// Parameterless constructor for ORMs like Entity Framework Core.
    /// </summary>
    protected AggregateRoot() { }
}