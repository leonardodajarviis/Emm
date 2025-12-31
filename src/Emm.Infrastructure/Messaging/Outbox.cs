using Emm.Application.Abstractions;
using Emm.Domain.Abstractions;
using Emm.Infrastructure.Data;
using LazyNet.Symphony.Interfaces;

namespace Emm.Infrastructure.Messaging;

public class Outbox : IOutbox
{
    private readonly XDbContext _context;
    private readonly IEventSerializer _serializer;
    public Outbox(XDbContext context, IEventSerializer serializer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }
    public void Enqueue(IDomainEvent @event)
    {
        var now = DateTime.UtcNow;

        _context.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = @event.GetType().AssemblyQualifiedName!,
            Payload = _serializer.Serialize(@event),
            CreatedAt = now
        });
    }

    public void EnqueueRange(IEnumerable<IDomainEvent> events)
    {
        var now = DateTime.UtcNow;
        foreach (var e in events)
        {
            _context.OutboxMessages.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = e.GetType().AssemblyQualifiedName!,
                Payload = _serializer.Serialize(e),
                CreatedAt = now
            });
        }
    }
}
