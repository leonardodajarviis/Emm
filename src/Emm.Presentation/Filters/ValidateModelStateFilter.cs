using Emm.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Emm.Presentation.Filters;

/// <summary>
/// Action filter to handle model state validation errors and return consistent error responses
/// </summary>
public class ValidateModelStateFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => new
                {
                    Field = x.Key,
                    Message = string.IsNullOrEmpty(e.ErrorMessage)
                        ? e.Exception?.Message ?? "Invalid value"
                        : e.ErrorMessage
                }))
                .ToList();

            var errorMessage = errors.Count == 1
                ? errors[0].Message
                : $"Validation failed for {errors.Count} field(s)";

            var details = errors.Select(e =>
                string.IsNullOrEmpty(e.Field)
                    ? e.Message
                    : $"{e.Field}: {e.Message}"
            ).ToList();

            var errorResponse = new ApiErrorResponse(
                code: "VALIDATION_ERROR",
                message: errorMessage,
                details: details
            );

            context.Result = new BadRequestObjectResult(errorResponse);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after execution
    }
}
