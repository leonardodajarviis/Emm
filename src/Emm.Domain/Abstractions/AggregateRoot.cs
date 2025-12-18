namespace Emm.Domain.Abstractions;

/// <summary>
/// Base class for aggregate roots that provides domain event management functionality.
/// Supports both immediate events (processed within transaction) and deferred events (processed via outbox).
/// </summary>
public abstract class AggregateRoot : IAggregateRoot
{
    /// <summary>
    /// Gets the unique identifier for this aggregate root.
    /// Uses UUID v7 which is time-ordered and database-friendly.
    /// </summary>
    public Guid Id { get; protected set; } = Guid.CreateVersion7();

    private readonly List<IDomainEvent> _domainEvents = [];
    private readonly List<IImmediateDomainEvent> _immediateEvents = [];

    /// <summary>
    /// Gets the collection of all domain events (both immediate and deferred) that have been raised by this aggregate.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Gets the collection of immediate domain events that must be processed within the current transaction.
    /// </summary>
    public IReadOnlyCollection<IImmediateDomainEvent> ImmediateEvents => _immediateEvents.AsReadOnly();

    /// <summary>
    /// Gets the collection of deferred domain events that will be processed asynchronously via outbox.
    /// </summary>
    public IReadOnlyCollection<IDeferredDomainEvent> DeferredEvents =>
        _domainEvents.OfType<IDeferredDomainEvent>().ToList().AsReadOnly();

    /// <summary>
    /// Adds a domain event to the aggregate's collection of events.
    /// Events implementing IImmediateDomainEvent will be tracked separately for immediate processing.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when domainEvent is null.</exception>
    public void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        // Track immediate events separately for transaction-scoped processing
        if (domainEvent is IImmediateDomainEvent immediateEvent)
        {
            _immediateEvents.Add(immediateEvent);
        }

        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a specific domain event from the aggregate's collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    /// <exception cref="ArgumentNullException">Thrown when domainEvent is null.</exception>
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Remove(domainEvent);

        // Also remove from immediate events if applicable
        if (domainEvent is IImmediateDomainEvent immediateEvent)
        {
            _immediateEvents.Remove(immediateEvent);
        }
    }

    /// <summary>
    /// Clears all domain events from the aggregate's collection.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
        _immediateEvents.Clear();
    }

    /// <summary>
    /// Protected method to raise domain events from within the aggregate.
    /// This is a convenience method for derived classes.
    /// </summary>
    /// <typeparam name="T">The type of domain event.</typeparam>
    /// <param name="domainEvent">The domain event to raise.</param>
    protected void Raise<T>(T domainEvent) where T : IDomainEvent
    {
        RaiseDomainEvent(domainEvent);
    }
}