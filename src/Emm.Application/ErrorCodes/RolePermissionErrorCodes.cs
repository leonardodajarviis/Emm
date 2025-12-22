namespace Emm.Application.ErrorCodes;

/// <summary>
/// Role & Permission error codes
/// </summary>
public static class RoleErrorCodes
{
    public const string NotFound = "ROLE_NOT_FOUND";
    public const string AlreadyExists = "ROLE_ALREADY_EXISTS";
    public const string InUse = "ROLE_IN_USE";
    public const string CannotDelete = "ROLE_CANNOT_DELETE";
}

/// <summary>
/// Permission error codes
/// </summary>
public static class PermissionErrorCodes
{
    public const string NotFound = "PERMISSION_NOT_FOUND";
    public const string Denied = "PERMISSION_DENIED";
    public const string AlreadyExists = "PERMISSION_ALREADY_EXISTS";
}

/// <summary>
/// Policy error codes
/// </summary>
public static class PolicyErrorCodes
{
    public const string NotFound = "POLICY_NOT_FOUND";
    public const string EvaluationFailed = "POLICY_EVALUATION_FAILED";
}
