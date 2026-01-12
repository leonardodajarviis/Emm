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
}

/// <summary>
/// Shift Log error codes
/// </summary>
public static class ShiftLogErrorCodes
{
    public const string NotFound = "SHIFT_LOG_NOT_FOUND";
    public const string AlreadyExists = "SHIFT_LOG_ALREADY_EXISTS";
    public const string CannotDelete = "SHIFT_LOG_CANNOT_DELETE";
    public const string ShiftLogMissmatch = "SHIFT_LOG_MISMATCH";
    public const string NotCurrentShiftLog = "SHIFT_LOG_NOT_CURRENT";
}
