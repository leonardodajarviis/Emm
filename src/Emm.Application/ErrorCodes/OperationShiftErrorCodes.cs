namespace Emm.Application.ErrorCodes;

/// <summary>
/// Operation Shift error codes
/// </summary>
public static class OperationShiftErrorCodes
{
}

/// <summary>
/// Shift Log error codes
/// </summary>
public static class ShiftLogErrorCodes
{
    public const string ShiftLogMissmatch = "ShiftLog.Missmatch";
    public const string ReadingOutOfRange = "ShiftLog.ReadingOutOfRange";
    public const string EventTimeOutOfRange = "ShiftLog.EventTimeOutOfRange";
    public const string AssetNotInOperationShift = "ShiftLog.AssetNotInOperationShift";
    public const string AssetBoxNotInOperationShift = "ShiftLog.AssetBoxNotInOperationShift";
    public const string AssetNotInIdleStatus = "ShiftLog.AssetNotInIdleStatus";
    public const string InvalidTime = "ShiftLog.InvalidShiftLogTime";
}
