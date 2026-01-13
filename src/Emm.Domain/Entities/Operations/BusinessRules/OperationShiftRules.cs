namespace Emm.Domain.Entities.Operations.BusinessRules;

public static class OperationShiftRules
{
    public const string EndTimeAfterStartTime = "OperationShift.EndTimeAfterStartTime";
    public const string ActualStartTimeBeforeActualEndTime = "OperationShift.ActualStartTimeBeforeActualEndTime";
    public const string ActualStartTimeAfterScheduledStartTime = "OperationShift.ActualStartTimeAfterScheduledStartTime";
    public const string CannotChangeScheduleInProgress = "OperationShift.CannotChangeScheduleInProgress";
}
