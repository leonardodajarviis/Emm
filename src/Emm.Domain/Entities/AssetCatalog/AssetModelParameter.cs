namespace Emm.Domain.Entities.AssetCatalog;

public class AssetModelParameter
{
    public Guid AssetModelId { get; private set; }
    public Guid ParameterId { get; private set; }

    public bool IsMaintenanceParameter { get; private set; }

    public AssetModelParameter(Guid parameterId)
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
