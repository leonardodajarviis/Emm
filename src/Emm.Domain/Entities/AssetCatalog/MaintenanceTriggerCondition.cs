namespace Emm.Domain.Entities.AssetCatalog;

public enum MaintenanceTriggerCondition
{
    None = 0,
    GreaterThan = 1,
    GreaterThanOrEqual = 2,
    LessThan = 3,
    LessThanOrEqual = 4,
    Equal = 5,
    NotEqual = 6,
    Between = 7,
    Outside = 8,
    OutOfRange = 9
}
