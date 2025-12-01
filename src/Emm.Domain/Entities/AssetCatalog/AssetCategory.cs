using Emm.Domain.Abstractions;
using Emm.Domain.DomainEvents.AssetCategoryEvents;
using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.AssetCatalog;

public class AssetCategory : AggregateRoot, IAuditableEntity
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    // IAuditableEntity implementation với private set để bảo vệ encapsulation
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public AssetCategory(string code, string name, string? description = null, bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Category code is required");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name is required");

        Code = code;
        Name = name;
        Description = description;
        IsActive = isActive;

        // Raise domain event
        Raise(new AssetCategoryCreatedEvent(
            AssetCategoryId: Id,
            Code: Code,
            Name: Name,
            Description: Description,
            IsActive: IsActive,
            OccurredOn: DateTime.UtcNow));
    }

    public void Update(string name, string? description, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name is required");

        Name = name;
        Description = description;
        IsActive = isActive;

        // Raise domain event
        Raise(new AssetCategoryUpdatedEvent(
            AssetCategoryId: Id,
            Name: Name,
            Description: Description,
            IsActive: IsActive,
            OccurredOn: DateTime.UtcNow));
    }

    private AssetCategory() { } // EF Core constructor
}
