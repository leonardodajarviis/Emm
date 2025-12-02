using Emm.Application.Abstractions;
using Emm.Domain.Abstractions;
using Emm.Infrastructure.Data;
using LazyNet.Symphony.Interfaces;

namespace Emm.Infrastructure.Messaging;

public class Outbox : IOutbox
{
    private readonly XDbContext _context;
    private readonly IEventSerializer _serializer;
    private readonly IMediator _mediator;

    public Outbox(XDbContext context, IEventSerializer serializer, IMediator mediator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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

    public async Task PublishImmediateAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        // Publish the event immediately to all handlers within the current transaction
        // Immediate events are NOT saved to outbox - they are processed synchronously
        await _mediator.Publish((object)@event, cancellationToken);
    }

    public async Task PublishImmediateRangeAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
    {
        if (events == null)
            throw new ArgumentNullException(nameof(events));

        var eventList = events.ToList();

        // Publish all events immediately to handlers within the current transaction
        // Immediate events are NOT saved to outbox - they are processed synchronously
        foreach (var @event in eventList)
        {
            await _mediator.Publish((object)@event, cancellationToken);
        }
    }
}
