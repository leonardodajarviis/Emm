namespace Emm.Presentation.Models;

public class ApiErrorResponse
{
    public ApiError Error { get; }

    public ApiErrorResponse(string code, string message, string? traceId = null, List<string>? details = null, string? stackTrace = null)
    {
        Error = new ApiError(code, message, traceId, details, stackTrace);
    }

    public ApiErrorResponse()
    {
        Error = new ApiError();
    }
}

public class ApiError
{
    public string Message { get; set; }
    public string Code { get; set; }
    public string? TraceId { get; set; }
    public List<string>? Details { get; set; }
    public string? StackTrace { get; set; }

    public ApiError(string code, string message, string? traceId = null, List<string>? details = null, string? stackTrace = null)
    {
        Message = message;
        Code = code;
        TraceId = traceId;
        Details = details;
        StackTrace = stackTrace;
    }

    public ApiError()
    {
        Message = "An error occurred.";
        Code = "UNKNOWN_ERROR";
    }
}