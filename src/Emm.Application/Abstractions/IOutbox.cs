using Emm.Domain.Abstractions;

namespace Emm.Application.Abstractions;

public interface IOutbox
{
    void Enqueue(IDomainEvent @event);
    void EnqueueRange(IEnumerable<IDomainEvent> events);

    /// <summary>
    /// Publishes an event immediately within the current transaction.
    /// The event will be published to handlers before the transaction commits.
    /// If any handler fails, the entire transaction will be rolled back.
    /// </summary>
    Task PublishImmediateAsync(IDomainEvent @event, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes multiple events immediately within the current transaction.
    /// All events will be published to handlers before the transaction commits.
    /// If any handler fails, the entire transaction will be rolled back.
    /// </summary>
    Task PublishImmediateRangeAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
}