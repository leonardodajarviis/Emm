using System.Net;
using System.Text.Json;
using Emm.Presentation.Models;
using Emm.Presentation.Options;
using Microsoft.Extensions.Options;

namespace Emm.Presentation.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly ErrorHandlingOptions _errorOptions;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IOptions<ErrorHandlingOptions> errorOptions)
    {
        _next = next;
        _logger = logger;
        _errorOptions = errorOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;
            _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}, Message: {Message}", traceId, ex.Message);
            await HandleExceptionAsync(context, ex, traceId);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, string traceId)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Collect exception details if enabled
        List<string>? exceptionDetails = null;
        string? stackTrace = null;

        if (_errorOptions.IncludeExceptionDetails)
        {
            exceptionDetails = CollectExceptionMessages(exception, _errorOptions.MaxExceptionDepth);
        }

        if (_errorOptions.IncludeStackTrace)
        {
            stackTrace = exception.StackTrace;
        }

        var errorResponse = new ApiErrorResponse(
            code: "INTERNAL_SERVER_ERROR",
            message: "An unexpected error occurred. Please try again later.",
            traceId: traceId,
            details: exceptionDetails,
            stackTrace: stackTrace
        );

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        await context.Response.WriteAsJsonAsync(errorResponse, jsonOptions);
    }

    private static List<string> CollectExceptionMessages(Exception exception, int maxDepth)
    {
        var messages = new List<string>();
        var currentException = exception;
        var depth = 0;

        while (currentException != null && depth < maxDepth)
        {
            var exceptionType = currentException.GetType().Name;
            var message = $"[{exceptionType}] {currentException.Message}";
            messages.Add(message);

            currentException = currentException.InnerException;
            depth++;
        }

        return messages;
    }
}
