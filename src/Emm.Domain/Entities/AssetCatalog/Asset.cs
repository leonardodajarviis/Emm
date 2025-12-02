using Emm.Domain.Abstractions;
using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.AssetCatalog;

public class Asset : AggregateRoot, IAuditableEntity
{
    private const int MaxParametersPerAsset = 100;
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public long AssetCategoryId { get; private set; }
    public string? AssetCategoryCode { get; private set; }
    public string? AssetCategoryName { get; private set; }
    public long AssetModelId { get; private set; }
    public string? AssetModelCode { get; private set; }
    public string? AssetModelName { get; private set; }
    public long AssetTypeId { get; private set; }
    public string? AssetTypeCode { get; private set; }
    public string? AssetTypeName { get; private set; }
    public long? AssetAdditionId { get; private set; }
    public long OrganizationUnitId { get; private set; }
    public long LocationId { get; private set; }
    public string? Description { get; private set; }
    public bool IsLooked { get; private set; }
    public AssetStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<AssetParameter> _parameters;
    public IReadOnlyCollection<AssetParameter> Parameters => _parameters;

    private Asset()
    {
        _parameters = [];
    }

    public Asset(
        string code,
        string displayName,
        long assetModelId,
        long organizationUnitId,
        long locationId,
        long? assetAdditionId,
        string? description = null)
    {
        ValidateCode(code);
        ValidateDisplayName(displayName);
        ValidateForeignKey(assetModelId, nameof(AssetModelId));
        ValidateForeignKey(organizationUnitId, nameof(OrganizationUnitId));
        ValidateForeignKey(locationId, nameof(LocationId));

        _parameters = [];

        Code = code;
        DisplayName = displayName;
        Description = description;
        AssetModelId = assetModelId;
        OrganizationUnitId = organizationUnitId;
        LocationId = locationId;
        AssetAdditionId = assetAdditionId;

        // RaiseDomainEvent(new AssetCreatedEvent(code, displayName));
    }

    public void Update(
        string displayName,
        string? description,
        long organizationUnitId,
        long locationId)
    {
        ValidateDisplayName(displayName);
        ValidateForeignKey(organizationUnitId, nameof(OrganizationUnitId));
        ValidateForeignKey(locationId, nameof(LocationId));

        var hasChanges = DisplayName != displayName
            || Description != description
            || OrganizationUnitId != organizationUnitId
            || LocationId != locationId;

        if (!hasChanges)
            return;

        DisplayName = displayName;
        Description = description;
        OrganizationUnitId = organizationUnitId;
        LocationId = locationId;

        // RaiseDomainEvent(new AssetUpdatedEvent(Id, DisplayName));
    }

    public void AddParameter(long parameterId, decimal value = 0, decimal valueToMaintenance = 0, string? parameterCode = null, string? parameterName = null, string? unit = null)
    {
        if (parameterId <= 0)
            throw new DomainException("Parameter ID must be greater than zero");

        if (_parameters.Count >= MaxParametersPerAsset)
            throw new DomainException($"Cannot exceed {MaxParametersPerAsset} parameters per asset");

        if (_parameters.Any(p => p.ParameterId == parameterId))
            throw new DomainException($"Parameter {parameterId} already exists in this asset");

        var parameter = new AssetParameter(
            parameterId,
            value,
            valueToMaintenance,
            parameterCode,
            parameterName,
            unit);
        _parameters.Add(parameter);

        // RaiseDomainEvent(new AssetParameterAddedEvent(Id, parameterId));
    }

    public void AddParameters(params long[] parameterIds)
    {
        if (parameterIds == null || parameterIds.Length == 0)
            return;

        foreach (var parameterId in parameterIds)
        {
            AddParameter(parameterId);
        }
    }

    public void Operate()
    {
        Status = AssetStatus.Operating;
    }

    public void Idle()
    {
        Status = AssetStatus.Idle;
    }

    public void UpdateParameterCurrentValue(long parameterId, decimal newValue)
    {
        if (parameterId <= 0)
            throw new DomainException("Parameter ID must be greater than zero");

        var parameter = _parameters.FirstOrDefault(p => p.ParameterId == parameterId);

        if (parameter == null)
            throw new DomainException($"Parameter {parameterId} not found in this asset");

        parameter.UpdateValue(newValue);

        // RaiseDomainEvent(new AssetParameterUpdatedEvent(Id, parameterId, newValue));
    }

    public void RemoveParameter(long parameterId)
    {
        if (parameterId <= 0)
            throw new DomainException("Parameter ID must be greater than zero");

        var parameter = _parameters.FirstOrDefault(p => p.ParameterId == parameterId);

        if (parameter == null)
            throw new DomainException($"Parameter {parameterId} not found in this asset");

        _parameters.Remove(parameter);
        // RaiseDomainEvent(new AssetParameterRemovedEvent(Id, parameterId));
    }

    // Validation methods
    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Asset code cannot be empty");

        if (code.Length > 50)
            throw new DomainException("Asset code cannot exceed 50 characters");
    }

    private static void ValidateDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new DomainException("Display name cannot be empty");

        if (displayName.Length > 200)
            throw new DomainException("Display name cannot exceed 200 characters");
    }

    private static void ValidateForeignKey(long foreignKeyId, string fieldName)
    {
        if (foreignKeyId <= 0)
            throw new DomainException($"{fieldName} must be greater than zero");
    }

}

public enum AssetStatus
{
    Maintenance = 1,     // Đang bảo trì
    Incident = 2,        // Đang gặp sự cố
    Operating = 3,       // Đang vận hành
    Idle = 4             // Đang rảnh
}
