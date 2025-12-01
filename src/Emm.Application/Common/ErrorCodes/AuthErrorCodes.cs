namespace Emm.Application.Common.ErrorCodes;

/// <summary>
/// Authentication & Authorization error codes
/// </summary>
public static class AuthErrorCodes
{
    public const string InvalidCredentials = "AUTH_INVALID_CREDENTIALS";
    public const string TokenExpired = "AUTH_TOKEN_EXPIRED";
    public const string TokenInvalid = "AUTH_TOKEN_INVALID";
    public const string RefreshTokenExpired = "AUTH_REFRESH_TOKEN_EXPIRED";
    public const string RefreshTokenInvalid = "AUTH_REFRESH_TOKEN_INVALID";
    public const string UserLocked = "AUTH_USER_LOCKED";
    public const string UserDisabled = "AUTH_USER_DISABLED";
    public const string SessionExpired = "AUTH_SESSION_EXPIRED";
    public const string SessionInvalid = "AUTH_SESSION_INVALID";
    public const string MultipleDeviceLogin = "AUTH_MULTIPLE_DEVICE_LOGIN";
    public const string PasswordExpired = "AUTH_PASSWORD_EXPIRED";
}
