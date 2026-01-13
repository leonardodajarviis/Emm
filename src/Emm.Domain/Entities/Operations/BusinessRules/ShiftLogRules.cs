namespace Emm.Domain.Entities.Operations.BusinessRules;

public static class ShiftLogRules
{
    public const string CannotSetBothAssetAndBox = "ShiftLog.CannotSetBothAssetAndBox";
    public const string BoxOrAssetMustExist = "ShiftLog.BoxOrAssetMustExist";
    public const string EndTimeCannotBeBeforeStartTime = "ShiftLog.EndTimeCannotBeBeforeStartTime";
}
