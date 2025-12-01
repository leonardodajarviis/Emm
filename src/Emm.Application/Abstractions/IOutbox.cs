using Emm.Domain.Abstractions;

namespace Emm.Application.Abstractions;

public interface IOutbox
{
    void Enqueue(IDomainEvent @event);
    void EnqueueRange(IEnumerable<IDomainEvent> events);
}