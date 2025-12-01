namespace Emm.Application.Common;

public static class ResultExtensions
{
    /// <summary>
    /// Chain multiple validation results together. Returns the first failure or success if all pass.
    /// </summary>
    public static Result Combine(params Result[] results)
    {
        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                return result;
            }
        }
        return Result.Success();
    }

    /// <summary>
    /// Chain validation to existing result
    /// </summary>
    public static async Task<Result> ThenValidateAsync(this Result result, Func<Task<Result>> nextValidation)
    {
        if (!result.IsSuccess)
        {
            return result;
        }

        return await nextValidation();
    }

    /// <summary>
    /// Chain validation to existing result (sync version)
    /// </summary>
    public static Result ThenValidate(this Result result, Func<Result> nextValidation)
    {
        if (!result.IsSuccess)
        {
            return result;
        }

        return nextValidation();
    }

    /// <summary>
    /// Execute action if result is success
    /// </summary>
    public static async Task<Result<T>> OnSuccessAsync<T>(this Result result, Func<Task<Result<T>>> onSuccess)
    {
        if (!result.IsSuccess)
        {
            return Result<T>.Failure(result.Error!.ErrorType, result.Error.Message, result.Error.Code);
        }

        return await onSuccess();
    }
}
