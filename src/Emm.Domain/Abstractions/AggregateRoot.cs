namespace Emm.Domain.Abstractions;

/// <summary>
/// Base class for aggregate roots that provides domain event management functionality.
/// </summary>
public abstract class AggregateRoot : IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets the collection of domain events that have been raised by this aggregate.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the aggregate's collection of events.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when domainEvent is null.</exception>
    public void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
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
    }

    /// <summary>
    /// Clears all domain events from the aggregate's collection.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
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