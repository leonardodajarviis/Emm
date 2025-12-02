# Architecture Decisions

## 1. Circular Dependency Resolution in DomainEventInterceptor

### Problem

Khi implement automatic domain event publishing, chúng ta gặp phải circular dependency:

```
DbContext → DomainEventInterceptor → IOutbox (Outbox) → IMediator
     ↑                                                        ↓
     └────────────────────────────────────────────────────────┘
                    (Handlers may inject DbContext)
```

**Chi tiết:**
1. `XDbContext` cần `DomainEventInterceptor` (constructor injection)
2. `DomainEventInterceptor` cần `IOutbox` để publish events
3. `Outbox` cần `IMediator` để publish immediate events
4. Event handlers (via `IMediator`) có thể inject `DbContext` để thực hiện operations

→ Tạo thành vòng lặp dependency!

### Solution: Service Scope Factory Pattern

Chúng ta sử dụng `IServiceScopeFactory` trong `DomainEventInterceptor` để create scope và resolve `IOutbox` at runtime thay vì constructor injection.

**Implementation:**

```csharp
public class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DomainEventInterceptor(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    private async Task PublishDomainEventsAsync(DbContext context, CancellationToken ct)
    {
        // Create a scope to resolve scoped services (since interceptor is singleton)
        using var scope = _serviceScopeFactory.CreateScope();
        var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();

        // Use outbox to publish events...
    }
}
```

**Registration:**

```csharp
// Interceptor is singleton to avoid recreating on every DbContext instantiation
services.AddSingleton<DomainEventInterceptor>();

// Outbox remains scoped (per request)
services.AddScoped<IOutbox, Outbox>();
```

**Why IServiceScopeFactory?**
- `DomainEventInterceptor` is a **singleton** (registered once for application lifetime)
- `IOutbox` is **scoped** (per request)
- Singleton cannot directly resolve scoped services from root provider
- `IServiceScopeFactory` allows creating a scope to resolve scoped services safely

### Why This Works

1. **Breaks Circular Dependency**:
   - `DomainEventInterceptor` không còn phụ thuộc trực tiếp vào `IOutbox`
   - Dependency được resolve lazily khi cần thiết

2. **Maintains Scope**:
   - Interceptor tạo scope mới mỗi khi `SaveChanges` được gọi
   - Scope này có thể resolve scoped services (`IOutbox`, `DbContext`, etc.)
   - Scope được dispose sau khi events được publish

3. **Thread Safety**:
   - `IServiceScopeFactory` is thread-safe
   - Mỗi SaveChanges operation tạo scope riêng
   - Không shared state giữa các requests

4. **Proper Scoping**:
   - Singleton interceptor → Safe to share across requests
   - Scoped services → Isolated per SaveChanges operation
   - Matches DbContext's scoped lifetime

### Trade-offs

#### ✅ Pros:
- Breaks circular dependency elegantly
- Minimal code changes
- Maintains proper service lifetimes
- No impact on functionality

#### ⚠️ Cons:
- Uses Service Locator anti-pattern (but limited to this specific case)
- Less explicit dependencies (not visible in constructor)
- Slightly harder to test (need to mock `IServiceProvider`)

### Alternative Solutions (Not Chosen)

#### 1. **Manual Event Publishing**
```csharp
// Application layer manually publishes events
var events = aggregate.DomainEvents;
await _unitOfWork.SaveChangesAsync();
foreach (var evt in events)
    await _mediator.Publish(evt);
```
**Rejected**: Loses automation benefit, error-prone (developers might forget)

#### 2. **Domain Event Dispatcher Service**
```csharp
services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
// Interceptor uses dispatcher instead of outbox
```
**Rejected**: Adds unnecessary abstraction layer

#### 3. **Post-SaveChanges Hook**
```csharp
public override async ValueTask<int> SavedChangesAsync(...)
{
    // Publish events AFTER SaveChanges
}
```
**Rejected**: Events published outside transaction scope, loses atomicity

### Conclusion

Service Locator pattern, while generally considered an anti-pattern, is the pragmatic choice here because:
- It's isolated to a single, infrastructure-level component
- The alternative solutions have worse trade-offs
- It enables a clean, automatic domain event publishing mechanism
- The circular dependency is genuinely unavoidable without this approach

This is a rare case where violating DI best practices results in a better overall architecture.

---

## 2. Immediate vs Deferred Domain Events

### Decision

Domain events are categorized into two types:
1. **Immediate Events** (`IImmediateDomainEvent`) - Published within transaction
2. **Deferred Events** (`IDeferredDomainEvent`) - Published asynchronously via outbox

### Rationale

Different events have different consistency requirements:
- **Strong Consistency**: Inventory validation must succeed/fail with order creation
- **Eventual Consistency**: Email notifications can be sent asynchronously

### Implementation

Events are automatically routed based on their interface:
```csharp
// Automatic routing in DomainEventInterceptor
var immediateEvents = aggregate.ImmediateEvents;  // Within transaction
var deferredEvents = aggregate.DeferredEvents;     // Via outbox
```

See [DOMAIN_EVENTS_USAGE.md](DOMAIN_EVENTS_USAGE.md) for usage guidelines.

---

## 3. Outbox Pattern with Optimistic Locking

### Decision

Use optimistic locking with `RowVersion` and distributed lock mechanism for outbox processing.

### Implementation

- Each `OutboxMessage` has `RowVersion`, `LockId`, and `LockedUntil`
- Processors claim messages using optimistic locking
- Retry with exponential backoff on failures

See [OutboxProcessor.cs](src/Emm.Infrastructure/Messaging/OutboxProcessor.cs) for details.

---

## References

- [Outbox Pattern](https://microservices.io/patterns/data/transactional-outbox.html)
- [Domain Events Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation)
- [Service Locator Anti-Pattern](https://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/)
