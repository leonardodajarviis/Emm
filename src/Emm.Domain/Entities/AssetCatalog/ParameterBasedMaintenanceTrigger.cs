namespace Emm.Domain.Entities.AssetCatalog;

public class ParameterBasedMaintenanceTrigger
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid MaintenancePlanDefinitionId { get; private set; }
    public Guid ParameterId { get; private set; }
    public decimal Value { get; private set; }
    public decimal PlusTolerance { get; private set; }
    public decimal MinusTolerance { get; private set; }
    public bool IsActive { get; private set; }

    public ParameterBasedMaintenanceTrigger(
        Guid parameterId,
        decimal value,
        decimal plusTolerance,
        decimal minusTolerance,
        bool isActive = true)
    {
        ParameterId = parameterId;
        Value = value;
        PlusTolerance = plusTolerance;
        MinusTolerance = minusTolerance;
        IsActive = isActive;
    }

    public void Update(
        decimal value,
        decimal plusTolerance,
        decimal minusTolerance,
        bool isActive)
    {
        Value = value;
        PlusTolerance = plusTolerance;
        MinusTolerance = minusTolerance;
        IsActive = isActive;
    }

    private ParameterBasedMaintenanceTrigger() { } // EF Core constructor
}
