namespace Emm.Application.ErrorCodes;

/// <summary>
/// Operation Shift error codes
/// </summary>
public static class ShiftErrorCodes
{
    public const string NotFound = "SHIFT_NOT_FOUND";
    public const string AlreadyExists = "SHIFT_ALREADY_EXISTS";
    public const string Overlap = "SHIFT_OVERLAP";
    public const string InUse = "SHIFT_IN_USE";
    public const string CannotDelete = "SHIFT_CANNOT_DELETE";

    // Status transition errors
    public const string CannotStart = "SHIFT_CANNOT_START";
    public const string CannotComplete = "SHIFT_CANNOT_COMPLETE";
    public const string CannotCancel = "SHIFT_CANNOT_CANCEL";
    public const string CannotPause = "SHIFT_CANNOT_PAUSE";
    public const string CannotResume = "SHIFT_CANNOT_RESUME";
    public const string CannotReschedule = "SHIFT_CANNOT_RESCHEDULE";
    public const string CannotReactivate = "SHIFT_CANNOT_REACTIVATE";
    public const string CannotMarkOverdue = "SHIFT_CANNOT_MARK_OVERDUE";
    public const string NotYetOverdue = "SHIFT_NOT_YET_OVERDUE";
    public const string InvalidStatusTransition = "SHIFT_INVALID_STATUS_TRANSITION";
}

/// <summary>
/// Shift Log error codes
/// </summary>
public static class ShiftLogErrorCodes
{
    public const string NotFound = "SHIFT_LOG_NOT_FOUND";
    public const string AlreadyExists = "SHIFT_LOG_ALREADY_EXISTS";
    public const string CannotDelete = "SHIFT_LOG_CANNOT_DELETE";
}
