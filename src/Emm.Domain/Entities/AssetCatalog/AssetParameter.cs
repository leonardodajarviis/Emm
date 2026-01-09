using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.AssetCatalog;

public class AssetParameter
{
    public Guid AssetId { get; private set; }
    public Guid ParameterId { get; private set; }
    public string ParameterCode { get; private set; } = null!;
    public string ParameterName { get; private set; } = null!;
    public ParameterType Type { get; private set; }
    public Guid UnitOfMeasureId { get; private set; }
    public decimal CurrentValue { get; private set; }
    public bool IsMaintenanceParameter { get; private set; }

    public AssetParameter(Guid parameterId, string parameterCode, string parameterName, Guid unitOfMeasureId, decimal value = 0, bool isMaintenanceParameter = false)
    {
        ParameterId = parameterId;
        CurrentValue = value;
        ParameterCode = parameterCode;
        ParameterName = parameterName;
        IsMaintenanceParameter = isMaintenanceParameter;
        UnitOfMeasureId = unitOfMeasureId;
    }

    public void Reading(decimal value)
    {
        switch (Type)
        {
            case ParameterType.Snapshot:
                CurrentValue = value;
                break;
            case ParameterType.Cumulative:
                CurrentValue += value;
                break;
            default:
                throw new DomainException("Unsupported parameter type");
        }
    }

    private AssetParameter()
    {
    }
}

public class AssetParameterMaintenance
{
    public Guid AssetId { get; private set; }
    public Guid ParameterId { get; private set; }
    public Guid MaintenancePlanId { get; private set; }
    public decimal ThresholdValue { get; private set; }
    public decimal PlusTolerance { get; private set; }
    public decimal MinusTolerance { get; private set; }

    public AssetParameterMaintenance(Guid parameterId, Guid maintenancePlanId, decimal thresholdValue, decimal plusTolerance, decimal minusTolerance)
    {
        ParameterId = parameterId;
        MaintenancePlanId = maintenancePlanId;
        ThresholdValue = thresholdValue;
        PlusTolerance = plusTolerance;
        MinusTolerance = minusTolerance;
    }

    private AssetParameterMaintenance()
    {
    } // EF Core constructor
}
