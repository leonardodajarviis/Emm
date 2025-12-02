namespace Emm.Domain.Abstractions;

/// <summary>
/// Represents an aggregate root in the domain.
/// Supports both immediate events (processed within transaction) and deferred events (processed via outbox).
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// Gets the collection of all domain events (both immediate and deferred) that have been raised by this aggregate.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Gets the collection of immediate domain events that must be processed within the current transaction.
    /// </summary>
    IReadOnlyCollection<IImmediateDomainEvent> ImmediateEvents { get; }

    /// <summary>
    /// Gets the collection of deferred domain events that will be processed asynchronously via outbox.
    /// </summary>
    IReadOnlyCollection<IDeferredDomainEvent> DeferredEvents { get; }

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