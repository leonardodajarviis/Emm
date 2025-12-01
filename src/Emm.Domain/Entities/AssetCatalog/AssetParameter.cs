namespace Emm.Domain.Entities.AssetCatalog;

public class AssetParameter
{
    public long AssetId { get; private set; }
    public long ParameterId { get; private set; }
    public string? ParameterCode { get; private set; }
    public string? ParameterName { get; private set; }
    public string? ParameterUnit { get; private set; }
    public decimal CurrentValue { get; private set; }

    public decimal ValueToMaintenance {get; private set;}

    public AssetParameter(long parameterId, decimal value,decimal valueToMaintenance, string? parameterCode = null, string? parameterName = null, string? parameterUnit = null)
    {
        ParameterId = parameterId;
        CurrentValue = value;
        ValueToMaintenance = valueToMaintenance;
        ParameterCode = parameterCode;
        ParameterName = parameterName;
        ParameterUnit = parameterUnit;
    }

    internal void UpdateValue(decimal newValue)
    {
        CurrentValue = newValue;
    }

    private AssetParameter()
    {
    }
}
