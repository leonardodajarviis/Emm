using Emm.Domain.Abstractions;
using Emm.Domain.Exceptions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.AssetCatalog;

public class AssetCategory : AggregateRoot, IAuditableEntity
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsCodeGenerated { get; private set; }
    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    public AssetCategory(string code, bool isCodeGenerated, string name, string? description = null, bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Category code is required");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name is required");

        Code = code;
        Name = name;
        Description = description;
        IsActive = isActive;
        IsCodeGenerated = isCodeGenerated;
    }

    public void Update(string name, string? description, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name is required");

        Name = name;
        Description = description;
        IsActive = isActive;
    }

    private AssetCategory() { } // EF Core constructor
}
