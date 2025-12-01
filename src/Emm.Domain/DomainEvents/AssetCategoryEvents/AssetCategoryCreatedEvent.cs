using Emm.Domain.Abstractions;

namespace Emm.Domain.DomainEvents.AssetCategoryEvents;

public record AssetCategoryCreatedEvent(
    long AssetCategoryId,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    DateTime OccurredOn) : IDomainEvent;
