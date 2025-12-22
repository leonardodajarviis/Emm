using Emm.Domain.Exceptions;

namespace Emm.Application.ErrorCodes;

/// <summary>
/// Maps Domain exceptions to Application error codes.
/// Domain layer không biết về error codes - đây là responsibility của Application layer.
/// </summary>
public static class DomainErrorCodeMapper
{
    /// <summary>
    /// Map DomainException sang error code
    /// </summary>
    public static (string ErrorCode, ErrorType ErrorType) Map(DomainException exception) => exception switch
    {
        EntityNotFoundException ex => (GetEntityNotFoundCode(ex.EntityName), ErrorType.NotFound),
        EntityAlreadyExistsException ex => (GetEntityAlreadyExistsCode(ex.EntityName, ex.PropertyName), ErrorType.Conflict),
        BusinessRuleViolationException ex => (GetBusinessRuleCode(ex.RuleName), ErrorType.Validation),
        InvalidEntityStateException ex => (GetInvalidStateCode(ex.EntityName), ErrorType.Validation),
        InvalidValueException ex => (GetInvalidValueCode(ex.PropertyName), ErrorType.Validation),
        Emm.Domain.Exceptions.UnauthorizedAccessException => (GeneralErrorCodes.Forbidden, ErrorType.Forbidden),
        EntityInUseException ex => (GetEntityInUseCode(ex.EntityName), ErrorType.Conflict),
        // Backward compatibility: base DomainException với message
        _ => MapFromMessage(exception.Message)
    };

    /// <summary>
    /// Fallback mapping từ message (backward compatibility cho code cũ)
    /// </summary>
    private static (string ErrorCode, ErrorType ErrorType) MapFromMessage(string message)
    {
        var lowerMessage = message.ToLowerInvariant();

        return lowerMessage switch
        {
            _ when lowerMessage.Contains("not found") => (GeneralErrorCodes.NotFound, ErrorType.NotFound),
            _ when lowerMessage.Contains("already exists") => (GeneralErrorCodes.Conflict, ErrorType.Conflict),
            _ when lowerMessage.Contains("required") => (ValidationErrorCodes.FieldRequired, ErrorType.Validation),
            _ when lowerMessage.Contains("invalid") => (ValidationErrorCodes.FieldInvalidValue, ErrorType.Validation),
            _ when lowerMessage.Contains("cannot") => (BusinessErrorCodes.OperationNotAllowed, ErrorType.Validation),
            _ => (GeneralErrorCodes.UnknownError, ErrorType.Internal)
        };
    }

    #region Entity Not Found Codes
    private static string GetEntityNotFoundCode(string entityName) => entityName switch
    {
        "User" => UserErrorCodes.NotFound,
        "Role" => RoleErrorCodes.NotFound,
        "Permission" => PermissionErrorCodes.NotFound,
        "Policy" => PolicyErrorCodes.NotFound,
        "Asset" => AssetErrorCodes.NotFound,
        "AssetType" => AssetTypeErrorCodes.NotFound,
        "AssetModel" => AssetModelErrorCodes.NotFound,
        "OperationShift" or "Shift" => ShiftErrorCodes.NotFound,
        "OperationShiftAsset" => "SHIFT_ASSET_NOT_FOUND",
        "ShiftLog" => ShiftLogErrorCodes.NotFound,
        "File" or "UploadedFile" => FileErrorCodes.NotFound,
        _ => GeneralErrorCodes.NotFound
    };
    #endregion

    #region Entity Already Exists Codes
    private static string GetEntityAlreadyExistsCode(string entityName, string? propertyName) => entityName switch
    {
        "User" when propertyName == "Email" => UserErrorCodes.EmailExists,
        "User" when propertyName == "Username" => UserErrorCodes.UsernameExists,
        "User" => UserErrorCodes.AlreadyExists,
        "Role" => RoleErrorCodes.AlreadyExists,
        "Permission" => PermissionErrorCodes.AlreadyExists,
        "Asset" when propertyName == "Code" => AssetErrorCodes.CodeExists,
        "Asset" => AssetErrorCodes.AlreadyExists,
        "AssetType" => AssetTypeErrorCodes.AlreadyExists,
        "AssetModel" => AssetModelErrorCodes.AlreadyExists,
        "OperationShift" or "Shift" => ShiftErrorCodes.AlreadyExists,
        "ShiftLog" => ShiftLogErrorCodes.AlreadyExists,
        _ => GeneralErrorCodes.Conflict
    };
    #endregion

    #region Entity In Use Codes
    private static string GetEntityInUseCode(string entityName) => entityName switch
    {
        "Role" => RoleErrorCodes.InUse,
        "AssetType" => AssetTypeErrorCodes.InUse,
        "AssetModel" => AssetModelErrorCodes.InUse,
        "Asset" => AssetErrorCodes.InUse,
        "OperationShift" or "Shift" => ShiftErrorCodes.InUse,
        _ => BusinessErrorCodes.DependencyExists
    };
    #endregion

    #region Business Rule Codes
    private static string GetBusinessRuleCode(string ruleName) => ruleName switch
    {
        // Asset rules
        "AssetCannotBeModifiedWhenDisposed" => "ASSET_CANNOT_MODIFY_DISPOSED",
        "AssetMustHaveType" => "ASSET_MUST_HAVE_TYPE",
        "AssetCodeMustBeUnique" => AssetErrorCodes.CodeExists,

        // Shift rules
        "ShiftCannotOverlap" => ShiftErrorCodes.Overlap,
        "ShiftMustHaveValidTime" => "SHIFT_INVALID_TIME",
        "ShiftCannotStartInFuture" => "SHIFT_CANNOT_START_IN_FUTURE",
        "ShiftEndTimeBeforeStartTime" => "SHIFT_END_TIME_BEFORE_START",
        "ShiftEndTimeMustBeAfterStartTime" => "SHIFT_END_TIME_MUST_BE_AFTER_START",
        "ShiftDurationExceeds24Hours" => "SHIFT_DURATION_EXCEEDS_24_HOURS",
        "CannotChangeScheduleInProgress" => "SHIFT_CANNOT_CHANGE_SCHEDULE_IN_PROGRESS",
        "OnlyOnePrimaryAssetAllowed" => "SHIFT_ONLY_ONE_PRIMARY_ASSET",

        // User rules
        "UserMustHaveAtLeastOneRole" => "USER_MUST_HAVE_ROLE",
        "UserCannotDeleteSelf" => "USER_CANNOT_DELETE_SELF",
        "PasswordMustMeetRequirements" => UserErrorCodes.PasswordTooWeak,

        // Auth rules
        "SessionExpired" => AuthErrorCodes.SessionExpired,
        "MultipleDeviceLoginNotAllowed" => AuthErrorCodes.MultipleDeviceLogin,

        // General
        _ => BusinessErrorCodes.RuleViolation
    };
    #endregion

    #region Invalid State Codes
    private static string GetInvalidStateCode(string entityName) => entityName switch
    {
        "Asset" => "ASSET_INVALID_STATE",
        "OperationShift" or "Shift" => "SHIFT_INVALID_STATE",
        "User" => "USER_INVALID_STATE",
        _ => BusinessErrorCodes.InvalidStateTransition
    };
    #endregion

    #region Invalid Value Codes
    private static string GetInvalidValueCode(string propertyName) => propertyName switch
    {
        "Email" => ValidationErrorCodes.EmailInvalid,
        "Phone" or "PhoneNumber" => ValidationErrorCodes.PhoneInvalid,
        "Url" or "Uri" => ValidationErrorCodes.UrlInvalid,
        "Date" or "DateTime" => ValidationErrorCodes.DateInvalid,
        _ => ValidationErrorCodes.FieldInvalidValue
    };
    #endregion
}
