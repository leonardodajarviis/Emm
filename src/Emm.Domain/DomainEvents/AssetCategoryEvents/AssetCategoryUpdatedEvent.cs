using Emm.Domain.Abstractions;

namespace Emm.Domain.DomainEvents.AssetCategoryEvents;

public record AssetCategoryUpdatedEvent(
    long AssetCategoryId,
    string Name,
    string? Description,
    bool IsActive,
    DateTime OccurredOn) : IDomainEvent;
