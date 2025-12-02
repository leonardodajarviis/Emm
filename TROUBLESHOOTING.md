# Troubleshooting Guide

## Common Issues and Solutions

### 1. "Cannot resolve scoped service from root provider"

**Error:**
```
System.InvalidOperationException: Cannot resolve scoped service 'Emm.Application.Abstractions.IOutbox' from root provider.
```

**Cause:**
- Singleton service trying to resolve scoped service directly
- In our case: `DomainEventInterceptor` (singleton) trying to resolve `IOutbox` (scoped)

**Solution:**
Use `IServiceScopeFactory` to create a scope:

```csharp
public class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private async Task PublishDomainEventsAsync(DbContext context, CancellationToken ct)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
        // Use outbox...
    }
}
```

**Registration:**
```csharp
services.AddSingleton<DomainEventInterceptor>();
services.AddScoped<IOutbox, Outbox>();
```

---

### 2. Circular Dependency Between DbContext and Services

**Error:**
```
System.InvalidOperationException: A circular dependency was detected
```

**Cause:**
```
DbContext → Interceptor → Service → Mediator → Handlers → DbContext
```

**Solution:**
Break the cycle using `IServiceScopeFactory`:
- Don't inject the service directly into interceptor
- Resolve it lazily when needed using scope factory

See [ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md) for details.

---

### 3. Domain Events Not Being Published

**Checklist:**
1. ✅ Does your aggregate inherit from `AggregateRoot`?
2. ✅ Are you calling `Raise()` or `RaiseDomainEvent()` to add events?
3. ✅ Is `DomainEventInterceptor` registered?
4. ✅ Is the interceptor added to `DbContext`?
5. ✅ Are you calling `SaveChangesAsync()`?

**Debug:**
```csharp
// Check if events were raised
var events = aggregate.DomainEvents;
Console.WriteLine($"Events count: {events.Count}");

// Check if interceptor is running
// Add breakpoint in DomainEventInterceptor.PublishDomainEventsAsync
```

---

### 4. Immediate Events Not Rolling Back Transaction

**Issue:**
Immediate event handler fails, but transaction doesn't rollback.

**Cause:**
Handler exception is being caught somewhere.

**Solution:**
Ensure exception propagates:
```csharp
public class MyImmediateEventHandler : INotificationHandler<MyEvent>
{
    public async Task Handle(MyEvent evt, CancellationToken ct)
    {
        // DON'T catch exceptions here if you want rollback
        await SomeOperation();

        // If operation fails, exception will bubble up
        // → Interceptor will fail
        // → SaveChanges will fail
        // → Transaction will rollback ✅
    }
}
```

---

### 5. Deferred Events Not Being Processed

**Checklist:**
1. ✅ Is `OutboxProcessor` registered as hosted service?
2. ✅ Is the application running (not just handling one request)?
3. ✅ Check `OutboxMessages` table - are messages there?
4. ✅ Check `ProcessedAt` column - are they being processed?
5. ✅ Check `Error` column - are there errors?

**Debug:**
```sql
-- Check pending messages
SELECT * FROM OutboxMessages WHERE ProcessedAt IS NULL;

-- Check failed messages
SELECT * FROM OutboxMessages WHERE Error IS NOT NULL;

-- Check processed messages
SELECT * FROM OutboxMessages WHERE ProcessedAt IS NOT NULL;
```

**Enable logging:**
```csharp
services.Configure<OutboxProcessorOptions>(cfg =>
{
    cfg.BatchSize = 10;
    cfg.IdleDelay = TimeSpan.FromSeconds(1); // Faster for debugging
});
```

---

### 6. Migration Issues with OutboxMessage

**Error:**
```
The entity type 'OutboxMessage' requires a primary key to be defined.
```

**Solution:**
Ensure `OutboxMessageConfiguration` is applied:
```csharp
// In XDbContext.OnModelCreating
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(XDbContext).Assembly);
}
```

Create migration:
```bash
dotnet ef migrations add AddOutboxPattern -p src/Emm.Infrastructure
dotnet ef database update -p src/Emm.Infrastructure
```

---

## Performance Tips

### 1. Optimize Outbox Batch Size

```csharp
services.Configure<OutboxProcessorOptions>(cfg =>
{
    cfg.BatchSize = 100; // Process more messages per batch
    cfg.IdleDelay = TimeSpan.FromSeconds(5); // Check less frequently
});
```

### 2. Index Optimization

Ensure indexes exist on `OutboxMessages`:
- `IX_OutboxMessages_Processing` (ProcessedAt, LockedUntil, CreatedAt)
- `IX_OutboxMessages_LockId`

### 3. Cleanup Old Messages

```csharp
// Periodically delete processed messages
DELETE FROM OutboxMessages
WHERE ProcessedAt IS NOT NULL
  AND ProcessedAt < DATEADD(day, -7, GETUTCDATE());
```

---

## Testing Tips

### Unit Testing with Domain Events

```csharp
[Fact]
public void WhenCreatingOrder_ShouldRaiseOrderCreatedEvent()
{
    // Arrange
    var order = Order.Create(customerId, amount);

    // Assert
    Assert.Single(order.DomainEvents);
    Assert.IsType<OrderCreatedEvent>(order.DomainEvents.First());
}
```

### Integration Testing with Outbox

```csharp
[Fact]
public async Task WhenSavingAggregate_ShouldEnqueueDeferredEvents()
{
    // Arrange
    var order = Order.Create(customerId, amount);
    _repository.Add(order);

    // Act
    await _unitOfWork.SaveChangesAsync();

    // Assert
    var messages = await _dbContext.OutboxMessages.ToListAsync();
    Assert.NotEmpty(messages);
}
```

---

## Useful Queries

### Monitor Outbox Performance

```sql
-- Average processing time per message
SELECT
    AVG(DATEDIFF(second, CreatedAt, ProcessedAt)) as AvgProcessingSeconds,
    COUNT(*) as TotalProcessed
FROM OutboxMessages
WHERE ProcessedAt IS NOT NULL;

-- Failed messages by error type
SELECT
    LEFT(Error, 100) as ErrorPrefix,
    COUNT(*) as Count
FROM OutboxMessages
WHERE Error IS NOT NULL
GROUP BY LEFT(Error, 100);
```

---

## Getting Help

1. Check [DOMAIN_EVENTS_USAGE.md](DOMAIN_EVENTS_USAGE.md) for usage examples
2. Check [ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md) for design rationale
3. Enable debug logging for `Emm.Infrastructure.Messaging`
4. Add breakpoints in `DomainEventInterceptor` and `OutboxProcessor`
