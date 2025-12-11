namespace Emm.Domain.Entities.AssetCatalog;

public class AssetModelParameter
{
    public long AssetModelId { get; private set; }
    public long ParameterId { get; private set; }

    public bool IsMaintenanceParameter { get; private set; }

    public AssetModelParameter(long parameterId)
    {
        ParameterId = parameterId;
    }

    public void MarkAsMaintenanceParameter()
    {
        IsMaintenanceParameter = true;
    }

    public void UnmarkAsMaintenanceParameter()
    {
        IsMaintenanceParameter = false;
    }

    private AssetModelParameter()
    {
    } // EF Core constructor
}
