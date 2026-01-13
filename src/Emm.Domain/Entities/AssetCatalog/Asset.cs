using Emm.Domain.Abstractions;
using Emm.Domain.Events.Asset;
using Emm.Domain.Exceptions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.AssetCatalog;

public class Asset : AggregateRoot, IAuditableEntity
{
    private const int MaxParametersPerAsset = 100;
    public NaturalKey Code { get; private set; }
    public bool IsCodeGenerated { get; private set; }
    public string DisplayName { get; private set; } = null!;
    public Guid AssetCategoryId { get; private set; }
    public string? AssetCategoryCode { get; private set; }
    public string? AssetCategoryName { get; private set; }
    public Guid AssetModelId { get; private set; }
    public string? AssetModelCode { get; private set; }
    public string? AssetModelName { get; private set; }
    public Guid AssetTypeId { get; private set; }
    public string? AssetTypeCode { get; private set; }
    public string? AssetTypeName { get; private set; }
    public Guid? AssetAdditionId { get; private set; }
    public Guid OrganizationUnitId { get; private set; }
    public Guid LocationId { get; private set; }
    public string? Description { get; private set; }
    public bool IsLooked { get; private set; }
    public bool IsActive { get; private set; }
    public AssetStatus Status { get; private set; }

    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    private readonly List<AssetParameter> _parameters;
    public IReadOnlyCollection<AssetParameter> Parameters => _parameters;

    private readonly List<AssetParameterMaintenance> _parameterMaintenances;
    public IReadOnlyCollection<AssetParameterMaintenance> ParameterMaintenances => _parameterMaintenances;

    private Asset()
    {
        _parameters = [];
        _parameterMaintenances = [];
        Status = AssetStatus.Idle;
    }

    public Asset(
        bool isCodeGenerated,
        NaturalKey code,
        string displayName,
        Guid assetModelId,
        Guid assetCategoryId,
        Guid assetTypeId,
        Guid organizationUnitId,
        Guid locationId,
        Guid? assetAdditionId,
        string? description = null)
    {
        _parameters = [];
        _parameterMaintenances = [];

        IsCodeGenerated = isCodeGenerated;
        Code = code;
        DisplayName = displayName;
        Description = description;
        AssetModelId = assetModelId;
        AssetCategoryId = assetCategoryId;
        AssetTypeId = assetTypeId;
        OrganizationUnitId = organizationUnitId;
        LocationId = locationId;
        AssetAdditionId = assetAdditionId;
        Status = AssetStatus.Idle;
        IsActive = true;

        // RaiseDomainEvent(new AssetCreatedEvent(Id, code, displayName));
    }

    public void MakeSnapshot(
        string assetModeCode,
        string assetModelName,
        string assetTypeCode,
        string assetTypeName,
        string assetCategoryCode,
        string assetCategoryName)
    {
        AssetModelCode = assetModeCode;
        AssetModelName = assetModelName;
        AssetTypeCode = assetTypeCode;
        AssetTypeName = assetTypeName;
        AssetCategoryCode = assetCategoryCode;
        AssetCategoryName = assetCategoryName;
    }

    public void Update(
        string displayName,
        string? description,
        Guid organizationUnitId,
        Guid locationId)
    {
        DisplayName = displayName;
        Description = description;
        OrganizationUnitId = organizationUnitId;
        LocationId = locationId;
    }

    public void AddParameter(
        Guid parameterId,
        string parameterCode,
        string parameterName,
        Guid unitOfMeasureId,
        decimal value = 0,
        bool isMaintenanceParameter = false)
    {
        if (_parameters.Count >= MaxParametersPerAsset)
            throw new DomainException($"Cannot exceed {MaxParametersPerAsset} parameters per asset");

        if (_parameters.Any(p => p.ParameterId == parameterId))
            throw new DomainException($"Parameter {parameterId} already exists in this asset");

        var parameter = new AssetParameter(
            parameterId,
            parameterCode,
            parameterName,
            unitOfMeasureId,
            value,
            isMaintenanceParameter
            );
        _parameters.Add(parameter);

        // RaiseDomainEvent(new AssetParameterAddedEvent(Id, parameterId));
    }

    public void AddParameterMaintenance(
        Guid parameterId,
        Guid maintenancePlanId,
        decimal thresholdValue,
        decimal plusTolerance,
        decimal minusTolerance)
    {
        if (_parameterMaintenances.Any(p => p.ParameterId == parameterId))
            throw new DomainException($"Parameter maintenance {parameterId} already exists in this asset");

        _parameterMaintenances.Add(new AssetParameterMaintenance(parameterId, maintenancePlanId, thresholdValue, plusTolerance, minusTolerance));
    }

    public void Operate()
    {
        Status = AssetStatus.Operating;
    }

    public void Idle()
    {
        Status = AssetStatus.Idle;
    }

    public void RecordParameter(Guid parameterId, decimal value)
    {
        var parameter = _parameters.FirstOrDefault(p => p.ParameterId == parameterId) ?? throw new DomainException($"Parameter {parameterId} not found in this asset");

        parameter.Reading(value);

        var maintenances = _parameterMaintenances.Where(p => p.ParameterId == parameterId);

        foreach (var maintenance in maintenances)
        {
            if (parameter.CurrentValue < (maintenance.ThresholdValue - maintenance.MinusTolerance) ||
                parameter.CurrentValue > (maintenance.ThresholdValue + maintenance.PlusTolerance))
            {
                Raise(new AssetParameterMaintenanceRequiredEvent(
                    Id,
                    parameterId,
                    maintenance.MaintenancePlanId,
                    parameter.CurrentValue,
                    maintenance.ThresholdValue,
                    maintenance.PlusTolerance,
                    maintenance.MinusTolerance));
            }
        }
    }

    public void RemoveParameter(Guid parameterId)
    {
        var parameter = _parameters.FirstOrDefault(p => p.ParameterId == parameterId) ?? throw new DomainException($"Parameter {parameterId} not found in this asset");

        _parameters.Remove(parameter);
    }
}
