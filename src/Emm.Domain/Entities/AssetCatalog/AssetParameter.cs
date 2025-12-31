namespace Emm.Domain.Entities.AssetCatalog;

public class AssetParameter
{
    public Guid AssetId { get; private set; }
    public Guid ParameterId { get; private set; }
    public string? ParameterCode { get; private set; }
    public string? ParameterName { get; private set; }
    public string? ParameterUnit { get; private set; }
    public decimal CurrentValue { get; private set; }

    public bool IsMaintenanceParameter {get; private set;}

    public decimal ValueToMaintenance {get; private set;}

    public AssetParameter(Guid parameterId, bool isMaintenanceParameter,  decimal value,decimal valueToMaintenance, string? parameterCode = null, string? parameterName = null, string? parameterUnit = null)
    {
        ParameterId = parameterId;
        CurrentValue = value;
        ValueToMaintenance = valueToMaintenance;
        ParameterCode = parameterCode;
        ParameterName = parameterName;
        ParameterUnit = parameterUnit;
        IsMaintenanceParameter = isMaintenanceParameter;
    }

    internal void UpdateValue(decimal newValue)
    {
        CurrentValue = newValue;
    }

    private AssetParameter()
    {
    }
}
