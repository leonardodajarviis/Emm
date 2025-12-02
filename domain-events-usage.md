# Domain Events Usage Guide

## Overview

Domain events trong hệ thống được chia thành 2 loại:

1. **Immediate Events** (`IImmediateDomainEvent`) - Được publish ngay lập tức trong transaction
2. **Deferred Events** (`IDeferredDomainEvent`) - Được publish sau bởi Outbox pattern

## When to Use Which Type?

### Use Immediate Events When:
- ✅ Event handler phải thành công/thất bại cùng với transaction
- ✅ Cần validate hoặc enforce business rules trong transaction
- ✅ Cần strong consistency (không chấp nhận eventual consistency)
- ✅ Side effects phải xảy ra atomically với main operation

**Examples:**
- Inventory validation khi tạo order
- Enforce business constraints across aggregates
- Critical validation logic

### Use Deferred Events When:
- ✅ Event handler không ảnh hưởng đến kết quả transaction
- ✅ Eventual consistency là chấp nhận được
- ✅ External integrations (email, SMS, third-party APIs)
- ✅ Non-critical notifications

**Examples:**
- Send welcome email
- Send notifications
- Update analytics/reporting
- Integration with external systems

## Example Usage

### 1. Define Events

```csharp
// Immediate Event - Must succeed with transaction
public record OrderCreatedEvent(Guid OrderId, Guid CustomerId, decimal TotalAmount)
    : IImmediateDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Deferred Event - Processed async via outbox
public record WelcomeEmailEvent(Guid CustomerId, string Email)
    : IDeferredDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
```

### 2. Raise Events in Aggregate

```csharp
public class Order : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }

    public static Order Create(Guid customerId, decimal amount)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            TotalAmount = amount
        };

        // This will be published IMMEDIATELY within transaction
        order.Raise(new OrderCreatedEvent(order.Id, customerId, amount));

        // This will be queued in outbox for async processing
        order.Raise(new WelcomeEmailEvent(customerId, "customer@example.com"));

        return order;
    }
}
```

### 3. Create Event Handlers

```csharp
// Handler for immediate event - runs in transaction
public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly IInventoryService _inventoryService;

    public async Task Handle(OrderCreatedEvent notification, CancellationToken ct)
    {
        // Validate inventory - if this fails, order creation will be rolled back
        await _inventoryService.ReserveInventory(notification.OrderId);

        // Any exception here will rollback the entire transaction
    }
}

// Handler for deferred event - runs async via outbox
public class WelcomeEmailEventHandler : INotificationHandler<WelcomeEmailEvent>
{
    private readonly IEmailService _emailService;

    public async Task Handle(WelcomeEmailEvent notification, CancellationToken ct)
    {
        // Send email - if this fails, it will retry via outbox
        await _emailService.SendWelcomeEmail(notification.CustomerId, notification.Email);

        // Failures here won't affect the original transaction
    }
}
```

### 4. Using in Application Layer

```csharp
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly IRepository<Order, Guid> _orderRepo;
    private readonly IUnitOfWork _unitOfWork;

    public async Task Handle(CreateOrderCommand command, CancellationToken ct)
    {
        // Create aggregate and raise events
        var order = Order.Create(command.CustomerId, command.Amount);

        // Add to repository
        _orderRepo.Add(order);

        // SaveChanges will:
        // 1. Save order to database
        // 2. Publish immediate events (OrderCreatedEvent) - runs in transaction
        // 3. Queue deferred events (WelcomeEmailEvent) to outbox
        // 4. Commit transaction
        await _unitOfWork.SaveChangesAsync(ct);

        // If OrderCreatedEventHandler fails → entire operation rolls back
        // If WelcomeEmailEventHandler fails later → will retry via outbox
    }
}
```

## Architecture Flow

### Immediate Events Flow:
```
1. SaveChangesAsync()
2. DomainEventInterceptor detects ImmediateEvents
3. Publish via Mediator BEFORE SaveChanges completes
4. If handler fails → Transaction rolls back
5. If handler succeeds → Transaction commits
```

### Deferred Events Flow:
```
1. SaveChangesAsync()
2. DomainEventInterceptor detects DeferredEvents
3. Add to OutboxMessages table
4. Transaction commits
5. OutboxProcessor picks up messages
6. Publish via Mediator asynchronously
7. If handler fails → Retry with exponential backoff
8. After max retries → Move to dead-letter
```

## Best Practices

1. **Keep Immediate Events Minimal**
   - Only use for critical business logic
   - Avoid external calls (API, email, etc.)
   - Keep handlers fast and reliable

2. **Use Deferred Events for Side Effects**
   - All notifications should be deferred
   - All external integrations should be deferred
   - Anything that can tolerate eventual consistency

3. **Event Naming Convention**
   - Use past tense: `OrderCreated`, `PaymentProcessed`
   - Be specific: `OrderShipped` not `OrderUpdated`

4. **Event Data**
   - Include minimal required data
   - Use IDs to fetch latest data in handler if needed
   - Events should be immutable (use `record`)

## Migration Strategy

If you have existing events without classification:

```csharp
// Old way (all events were deferred)
public record OrderCreatedEvent : IDomainEvent { ... }

// New way - explicitly choose type
public record OrderCreatedEvent : IImmediateDomainEvent { ... }
// or
public record OrderCreatedEvent : IDeferredDomainEvent { ... }

// Backward compatible: plain IDomainEvent is treated as deferred
public record OrderCreatedEvent : IDomainEvent { ... }  // Still works as deferred
```
