using Emm.Application.Common;
using Emm.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Extensions;
public static class ResultToActionResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess && result.Value is null)
            return new NoContentResult();
        else
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.Value);
        }

        var errorResponse =
            result.Error is not null ? new ApiErrorResponse(result.Error.Code, result.Error.Message) : new ApiErrorResponse();

        return result.Error?.ErrorType switch
        {
            ErrorType.NotFound => new NotFoundObjectResult(errorResponse),
            ErrorType.Validation => new BadRequestObjectResult(errorResponse),
            ErrorType.Conflict => new ConflictObjectResult(errorResponse),
            ErrorType.Unauthorized => new UnauthorizedObjectResult(errorResponse),
            ErrorType.Forbidden => new ObjectResult(errorResponse) { StatusCode = 403 },
            ErrorType.Invalid => new BadRequestObjectResult(errorResponse),
            _ => new ObjectResult(errorResponse) { StatusCode = 500 }
        };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        var errorResponse =
            result.Error is not null ? new ApiErrorResponse(result.Error.Code, result.Error.Message) : new ApiErrorResponse();

        return result.Error?.ErrorType switch
        {
            ErrorType.NotFound => new NotFoundObjectResult(errorResponse),
            ErrorType.Validation => new BadRequestObjectResult(errorResponse),
            ErrorType.Conflict => new ConflictObjectResult(errorResponse),
            ErrorType.Unauthorized => new UnauthorizedObjectResult(errorResponse),
            ErrorType.Forbidden => new ObjectResult(errorResponse) { StatusCode = 403 },
            ErrorType.Invalid => new BadRequestObjectResult(errorResponse),
            _ => new ObjectResult(errorResponse) { StatusCode = 500 }
        };
    }
}
