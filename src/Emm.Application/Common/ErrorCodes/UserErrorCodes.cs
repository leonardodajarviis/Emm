namespace Emm.Application.Common.ErrorCodes;

/// <summary>
/// User management error codes
/// </summary>
public static class UserErrorCodes
{
    public const string NotFound = "USER_NOT_FOUND";
    public const string AlreadyExists = "USER_ALREADY_EXISTS";
    public const string EmailExists = "USER_EMAIL_EXISTS";
    public const string UsernameExists = "USER_USERNAME_EXISTS";
    public const string InvalidEmail = "USER_INVALID_EMAIL";
    public const string InvalidPassword = "USER_INVALID_PASSWORD";
    public const string PasswordMismatch = "USER_PASSWORD_MISMATCH";
    public const string OldPasswordIncorrect = "USER_OLD_PASSWORD_INCORRECT";
    public const string PasswordTooWeak = "USER_PASSWORD_TOO_WEAK";
    public const string ProfileUpdateFailed = "USER_PROFILE_UPDATE_FAILED";
}
