namespace Emm.Domain.Entities.AssetCatalog;

public class MaintenancePlanJobStepDefinition
{
    public long Id { get; private set; }
    public long MaintenancePlanDefinitionId { get; private set; }
    public long? OrganizationUnitId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Note { get; private set; }
    public int Order { get; private set; }

    public MaintenancePlanJobStepDefinition(string name, long? organizationUnitId, string? note, int order)
    {
        Name = name;
        Note = note;
        OrganizationUnitId = organizationUnitId;
        Order = order;
    }

    public void Update(string name, string? note, int order)
    {
        Name = name;
        Note = note;
        Order = order;
    }

    private MaintenancePlanJobStepDefinition()
    {
    } // EF Core constructor
}
