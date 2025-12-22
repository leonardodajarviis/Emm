namespace Emm.Application.ErrorCodes;

/// <summary>
/// Business logic error codes
/// </summary>
public static class BusinessErrorCodes
{
    public const string OperationNotAllowed = "OPERATION_NOT_ALLOWED";
    public const string InvalidStateTransition = "INVALID_STATE_TRANSITION";
    public const string ResourceLocked = "RESOURCE_LOCKED";
    public const string QuotaExceeded = "QUOTA_EXCEEDED";
    public const string DependencyExists = "DEPENDENCY_EXISTS";
    public const string InvalidOperation = "INVALID_OPERATION";
    public const string PreconditionFailed = "PRECONDITION_FAILED";
    public const string RuleViolation = "BUSINESS_RULE_VIOLATION";
    public const string InvalidState = "INVALID_STATE";
}
