using Emm.Domain.Abstractions;

namespace Emm.Domain.Events;

public record VoidEvent : IDeferredDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
