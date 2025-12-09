using Emm.Domain.Abstractions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.AssetCatalog;

public class AssetType : AggregateRoot, IAuditableEntity
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsCodeGenerated { get; private set; }
    public long AssetCategoryId { get; private set; }
    private readonly List<AssetTypeParameter> _parameters = [];
    public IReadOnlyCollection<AssetTypeParameter> Parameters => _parameters.AsReadOnly();

    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;


    private AssetType() { } // EF Core constructor

    public AssetType(bool isCodeGenerated ,string code, string name, long assetCategoryId, string? description = null, bool isActive = true)
    {
        Code = code;
        Name = name;
        AssetCategoryId = assetCategoryId;
        Description = description;
        IsActive = isActive;
        IsCodeGenerated = isCodeGenerated;
    }

    public void Update(string name, long assetCategoryId, string? description, bool isActive)
    {
        Name = name;
        AssetCategoryId = assetCategoryId;
        Description = description;
        IsActive = isActive;
    }

    public void AddParameter(long parameterId)
    {
        var assetTypeParameter = new AssetTypeParameter(parameterId);
        _parameters.Add(assetTypeParameter);
    }
    public void AddParameters(params long[] parameterIds)
    {
        ArgumentNullException.ThrowIfNull(parameterIds);

        foreach (var parameterId in parameterIds)
        {
            AddParameter(parameterId);
        }
    }

    public void RemoveParameter(long parameterId)
    {
        var parameter = _parameters.FirstOrDefault(p => p.ParameterId == parameterId);
        if (parameter != null)
        {
            _parameters.Remove(parameter);
        }
    }
}

public class AssetTypeParameter
{
    public long AssetTypeId { get; private set; }
    public long ParameterId { get; private set; }
    public AssetTypeParameter(long parameterId)
    {
        ParameterId = parameterId;
    }

    private AssetTypeParameter() { } // EF Core constructor
}
