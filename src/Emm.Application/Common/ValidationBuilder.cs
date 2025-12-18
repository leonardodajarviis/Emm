using Emm.Application.Abstractions;

namespace Emm.Application.Common;

/// <summary>
/// Fluent validation builder for complex FK validations
/// </summary>
public class ValidationBuilder
{
    private readonly IForeignKeyValidator _fkValidator;
    private readonly List<Func<CancellationToken, Task<Result>>> _validations = [];

    public ValidationBuilder(IForeignKeyValidator fkValidator)
    {
        _fkValidator = fkValidator;
    }

    /// <summary>
    /// Add FK validation for single ID
    /// </summary>
    public ValidationBuilder ValidateForeignKey<TEntity>(Guid id, string? entityName = null) where TEntity : class
    {
        _validations.Add(ct => _fkValidator.ValidateAsync<TEntity>(id, entityName, ct));
        return this;
    }

    /// <summary>
    /// Add FK validation for nullable ID
    /// </summary>
    public ValidationBuilder ValidateForeignKey<TEntity>(Guid? id, string? entityName = null) where TEntity : class
    {
        if (id.HasValue)
        {
            _validations.Add(ct => _fkValidator.ValidateAsync<TEntity>(id.Value, entityName, ct));
        }
        return this;
    }

    /// <summary>
    /// Add FK validation for multiple IDs
    /// </summary>
    public ValidationBuilder ValidateForeignKeys<TEntity>(IEnumerable<Guid>? ids, string? entityName = null) where TEntity : class
    {
        if (ids?.Any() == true)
        {
            _validations.Add(ct => _fkValidator.ValidateManyAsync<TEntity>(ids, entityName, ct));
        }
        return this;
    }

    /// <summary>
    /// Add custom validation
    /// </summary>
    public ValidationBuilder AddCustomValidation(Func<Result> validation)
    {
        _validations.Add(_ => Task.FromResult(validation()));
        return this;
    }

    /// <summary>
    /// Add custom async validation
    /// </summary>
    public ValidationBuilder AddCustomValidation(Func<Task<Result>> validation)
    {
        _validations.Add(_ => validation());
        return this;
    }

    /// <summary>
    /// Add custom async validation with cancellation token support
    /// </summary>
    public ValidationBuilder AddCustomValidation(Func<CancellationToken, Task<Result>> validation)
    {
        _validations.Add(validation);
        return this;
    }

    /// <summary>
    /// Execute all validations and return first failure or success
    /// </summary>
    public async Task<Result> ValidateAsync(CancellationToken cancellationToken = default)
    {
        foreach (var validation in _validations)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await validation(cancellationToken);
            if (!result.IsSuccess)
            {
                return result;
            }
        }
        return Result.Success();
    }

    /// <summary>
    /// Execute validations and continue with action if all pass
    /// </summary>
    public async Task<Result<T>> ValidateAndExecuteAsync<T>(
        Func<Task<Result<T>>> action,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateAsync(cancellationToken);
        if (!validationResult.IsSuccess)
        {
            return Result<T>.Failure(
                validationResult.Error!.ErrorType,
                validationResult.Error.Message,
                validationResult.Error.Code);
        }

        return await action();
    }
}

/// <summary>
/// Extension to create ValidationBuilder easily
/// </summary>
public static class ValidationBuilderExtensions
{
    public static ValidationBuilder CreateValidator(this IForeignKeyValidator fkValidator)
    {
        return new ValidationBuilder(fkValidator);
    }
}
