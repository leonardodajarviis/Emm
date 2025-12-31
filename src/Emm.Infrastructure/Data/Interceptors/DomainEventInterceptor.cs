using Emm.Application.Abstractions;
using Emm.Domain.Abstractions;
using Emm.Infrastructure.Messaging;
using LazyNet.Symphony.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Emm.Infrastructure.Data.Interceptors;

/// <summary>
/// Handles Domain Events during SaveChanges using a safe DDD + Outbox approach.
///
/// Rules:
/// - Immediate events are published synchronously (NO DbContext mutation)
/// - Deferred events are queued to Outbox (same transaction)
/// - NO SaveChanges inside interceptor (EF Core best practice)
/// </summary>
public sealed class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly IServiceScopeFactory _scopeFactory;

    // Prevent re-entrancy per async flow
    private static readonly AsyncLocal<bool> _handling = new();

    public DomainEventInterceptor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (_handling.Value || eventData.Context is null)
            return result;

        _handling.Value = true;
        try
        {
            await DispatchDomainEventsAsync(eventData.Context);
        }
        finally
        {
            _handling.Value = false;
        }

        return result;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (_handling.Value || eventData.Context is null)
            return result;

        _handling.Value = true;
        try
        {
            DispatchDomainEventsAsync(eventData.Context)
                .GetAwaiter()
                .GetResult();
        }
        finally
        {
            _handling.Value = false;
        }

        return result;
    }

    private Task DispatchDomainEventsAsync(DbContext context)
    {
        var aggregates = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        if (aggregates.Count == 0)
            return Task.CompletedTask;

        var deferredEvents = aggregates
            .SelectMany(a => a.DeferredEvents)
            .ToList();

        using var scope = _scopeFactory.CreateScope();
        var serialize = scope.ServiceProvider.GetRequiredService<IEventSerializer>();

        if (deferredEvents.Count > 0)
        {
            var xcontext = (XDbContext)context;

            foreach (var defEvent in deferredEvents)
            {
                xcontext.OutboxMessages.Add(new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = defEvent.GetType().AssemblyQualifiedName!,
                    Payload = serialize.Serialize(defEvent),
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        return Task.CompletedTask;
    }

}
