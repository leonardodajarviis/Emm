namespace Emm.Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public ErrorResult? Error { get; }

    public object? Value { get; set; }

    protected Result(bool isSuccess, object? value = null, ErrorResult? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }

    public static Result Success(object? value = null) => new(true, value);

    public static Result Failure(ErrorType errorType, string? errorMessage = null, string? errorCode = null) =>
        new(false, new ErrorResult(errorType, errorCode, errorMessage));

    public static Result NotFound(string? errorMessage = null, string? errorCode = null) =>
        new(false, new ErrorResult(ErrorType.NotFound, errorCode, errorMessage));

    public static Result Validation(string? errorMessage = null, string? errorCode = null) =>
        new(false, new ErrorResult(ErrorType.Validation, errorCode, errorMessage));

    public static Result Conflict(string? errorMessage = null, string? errorCode = null) =>
        new(false, new ErrorResult(ErrorType.Conflict, errorCode, errorMessage));

    public static Result Unauthorized(string? errorMessage = null, string? errorCode = null) =>
        new(false, new ErrorResult(ErrorType.Unauthorized, errorCode, errorMessage));

    public static Result Invalid(string? errorMessage = null, string? errorCode = null) =>
        new(false, new ErrorResult(ErrorType.Invalid, errorCode, errorMessage));

    public static Result Forbidden(string? errorMessage = null, string? errorCode = null) =>
        new(false, new ErrorResult(ErrorType.Forbidden, errorCode, errorMessage));

    public static Result Internal(string? errorMessage = null, string? errorCode = null) =>
        new(false, new ErrorResult(ErrorType.Internal, errorCode, errorMessage));
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public ErrorResult? Error { get; }

    public Result() { }

    protected Result(bool isSuccess, T? value, ErrorResult? error = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value);

    public static Result<T> Failure(ErrorType errorType, string? errorMessage = null, string? errorCode = null) =>
        new(false, default, new ErrorResult(errorType, errorCode, errorMessage));

    public static Result<T> NotFound(string? errorMessage = null, string? errorCode = null) =>
        new(false, default, new ErrorResult(ErrorType.NotFound, errorCode, errorMessage));

    public static Result<T> Validation(string? errorMessage = null, string? errorCode = null) =>
        new(false, default, new ErrorResult(ErrorType.Validation, errorCode, errorMessage));

    public static Result<T> Conflict(string? errorMessage = null, string? errorCode = null) =>
        new(false, default, new ErrorResult(ErrorType.Conflict, errorCode, errorMessage));

    public static Result<T> Unauthorized(string? errorMessage = null, string? errorCode = null) =>
        new(false, default, new ErrorResult(ErrorType.Unauthorized, errorCode, errorMessage));

    public static Result<T> Invalid(string? errorMessage = null, string? errorCode = null) =>
        new(false, default, new ErrorResult(ErrorType.Invalid, errorCode, errorMessage));

    public static Result<T> Forbidden(string? errorMessage = null, string? errorCode = null) =>
        new(false, default, new ErrorResult(ErrorType.Forbidden, errorCode, errorMessage));

    public static Result<T> Internal(string? errorMessage = null, string? errorCode = null) =>
        new(false, default, new ErrorResult(ErrorType.Internal, errorCode, errorMessage));
}

public class ErrorResult
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType ErrorType { get; }

    public ErrorResult(ErrorType errorType, string? code, string? message = null)
    {
        Code = code ?? ErrorTypeToString(errorType);
        Message = message ?? "An error occurred.";
        ErrorType = errorType;
    }

    private static string ErrorTypeToString(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.NotFound => "NOT_FOUND",
            ErrorType.Validation => "VALIDATION_ERROR",
            ErrorType.Conflict => "CONFLICT",
            ErrorType.Unauthorized => "UNAUTHORIZED",
            ErrorType.Forbidden => "FORBIDDEN",
            ErrorType.Internal => "INTERNAL_SERVER_ERROR",
            _ => "UNKNOWN_ERROR"
        };
    }
}

public enum ErrorType
{
    NotFound,
    Validation,
    Conflict,
    Unauthorized,
    Invalid,
    Forbidden,
    Internal
}


