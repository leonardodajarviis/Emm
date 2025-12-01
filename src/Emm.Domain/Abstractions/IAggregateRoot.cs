namespace Emm.Domain.Abstractions;

/// <summary>
/// Represents an aggregate root in the domain.
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// Gets the collection of domain events that have been raised by this aggregate.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    
    /// <summary>
    /// Adds a domain event to the aggregate's collection of events.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    void RaiseDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// Removes a specific domain event from the aggregate's collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    void RemoveDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// Clears all domain events from the aggregate's collection.
    /// </summary>
    void ClearDomainEvents();
}