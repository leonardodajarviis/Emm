namespace Emm.Domain.Entities.AssetCatalog;

public class AssetParameter
{
    public Guid AssetId { get; private set; }
    public Guid ParameterId { get; private set; }
    public string ParameterCode { get; private set; } = null!;
    public string ParameterName { get; private set; } = null!;
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

    internal void UpdateValue(decimal newValue)
    {
        CurrentValue = newValue;
    }

    private AssetParameter()
    {
    }
}
