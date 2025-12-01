namespace Emm.Application.Common;

// Re-export all error codes for convenient access
// Usage: using Emm.Application.Common.ErrorCodes;
// Then: UserErrorCodes.NotFound, AuthErrorCodes.InvalidCredentials, etc.

/// <summary>
/// Helper class to create Result with standard error codes
/// </summary>
public static class ResultWithCode
{
    public static Result NotFound(string errorCode, string? message = null) =>
        Result.NotFound(message, errorCode);

    public static Result Validation(string errorCode, string? message = null) =>
        Result.Validation(message, errorCode);

    public static Result Conflict(string errorCode, string? message = null) =>
        Result.Conflict(message, errorCode);

    public static Result Unauthorized(string errorCode, string? message = null) =>
        Result.Unauthorized(message, errorCode);

    public static Result Forbidden(string errorCode, string? message = null) =>
        Result.Forbidden(message, errorCode);

    public static Result<T> NotFound<T>(string errorCode, string? message = null) =>
        Result<T>.NotFound(message, errorCode);

    public static Result<T> Validation<T>(string errorCode, string? message = null) =>
        Result<T>.Validation(message, errorCode);

    public static Result<T> Conflict<T>(string errorCode, string? message = null) =>
        Result<T>.Conflict(message, errorCode);

    public static Result<T> Unauthorized<T>(string errorCode, string? message = null) =>
        Result<T>.Unauthorized(message, errorCode);

    public static Result<T> Forbidden<T>(string errorCode, string? message = null) =>
        Result<T>.Forbidden(message, errorCode);
}
