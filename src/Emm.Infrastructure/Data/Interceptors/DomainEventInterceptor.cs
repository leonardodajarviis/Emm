using Emm.Application.Abstractions;
using Emm.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Emm.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor that handles publishing domain events when SaveChanges is called.
///
/// ARCHITECTURE DECISION:
/// - Uses BOTH SavingChanges (for immediate events) and SavedChanges (for deferred events)
/// - Immediate events are published BEFORE SaveChanges completes (within transaction)
/// - Deferred events are queued to outbox AFTER SaveChanges completes (ensuring they're persisted)
///
/// Note: Uses IServiceScopeFactory to resolve scoped IOutbox to avoid circular dependency
/// (DbContext → Interceptor → Outbox → Mediator → Handlers → DbContext)
/// Since this interceptor is a singleton, it must create a scope to resolve scoped services.
/// </summary>
public class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DomainEventInterceptor(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    /// <summary>
    /// BEFORE SaveChanges: Publish immediate events within the transaction
    /// </summary>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            await PublishImmediateEventsAsync(eventData.Context, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// BEFORE SaveChanges: Publish immediate events within the transaction (sync version)
    /// </summary>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context != null)
        {
            PublishImmediateEventsAsync(eventData.Context, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// AFTER SaveChanges: Queue deferred events to outbox for background processing
    /// </summary>
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            await QueueDeferredEventsAsync(eventData.Context, cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// AFTER SaveChanges: Queue deferred events to outbox for background processing (sync version)
    /// </summary>
    public override int SavedChanges(
        SaveChangesCompletedEventData eventData,
        int result)
    {
        if (eventData.Context != null)
        {
            QueueDeferredEventsAsync(eventData.Context, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        return base.SavedChanges(eventData, result);
    }

    /// <summary>
    /// Publishes immediate events within the current transaction.
    /// These events must be processed before the transaction commits.
    /// </summary>
    private async Task PublishImmediateEventsAsync(DbContext context, CancellationToken cancellationToken)
    {
        // Get all aggregate roots that have immediate events
        var aggregateRoots = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(e => e.Entity.ImmediateEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        if (!aggregateRoots.Any())
            return;

        // Create a scope to resolve scoped services (since interceptor is singleton)
        using var scope = _serviceScopeFactory.CreateScope();
        var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();

        // Publish immediate events for each aggregate root
        foreach (var aggregateRoot in aggregateRoots)
        {
            var immediateEvents = aggregateRoot.ImmediateEvents.ToList();
            if (immediateEvents.Any())
            {
                await outbox.PublishImmediateRangeAsync(immediateEvents, cancellationToken);
            }
        }

        // Note: We don't clear events here because SavedChanges hook still needs them
    }

    /// <summary>
    /// Queues deferred events to outbox AFTER the main SaveChanges has completed.
    /// This ensures the outbox messages are persisted in the same transaction.
    /// </summary>
    private async Task QueueDeferredEventsAsync(DbContext context, CancellationToken cancellationToken)
    {
        // Get all aggregate roots that have deferred events
        var aggregateRoots = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(e => e.Entity.DeferredEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        if (!aggregateRoots.Any())
        {
            // Even if no deferred events, clear all events from aggregates that had immediate events
            ClearAllDomainEvents(context);
            return;
        }

        // Create a scope to resolve scoped services (since interceptor is singleton)
        using var scope = _serviceScopeFactory.CreateScope();
        var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();

        // Queue deferred events to outbox
        foreach (var aggregateRoot in aggregateRoots)
        {
            var deferredEvents = aggregateRoot.DeferredEvents.ToList();
            if (deferredEvents.Any())
            {
                outbox.EnqueueRange(deferredEvents);
            }
        }

        // Save the outbox messages (this will trigger another SaveChanges)
        await context.SaveChangesAsync(cancellationToken);

        // Clear all events from all aggregates
        ClearAllDomainEvents(context);
    }

    /// <summary>
    /// Clears domain events from all aggregate roots in the context
    /// </summary>
    private void ClearAllDomainEvents(DbContext context)
    {
        var aggregateRoots = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var aggregateRoot in aggregateRoots)
        {
            aggregateRoot.ClearDomainEvents();
        }
    }
}
