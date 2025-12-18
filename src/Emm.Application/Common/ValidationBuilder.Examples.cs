using Emm.Application.Abstractions;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Common;

/// <summary>
/// Examples demonstrating ValidationBuilder usage with proper CancellationToken support
/// </summary>
public static class ValidationBuilderExamples
{
    /// <summary>
    /// Example 1: Basic FK validation with CancellationToken
    /// </summary>
    public static async Task<Result<object>> Example1_BasicValidation(
        IForeignKeyValidator fkValidator,
        Guid userId,
        Guid organizationUnitId,
        CancellationToken cancellationToken)
    {
        return await fkValidator.CreateValidator()
            .ValidateForeignKey<User>(userId, "User")
            .ValidateForeignKey<OrganizationUnit>(organizationUnitId, "Organization Unit")
            .ValidateAndExecuteAsync(async () =>
            {
                // Business logic here
                await Task.CompletedTask;
                return Result<object>.Success(new { Message = "Validation passed!" });
            }, cancellationToken);

        // CancellationToken được pass qua:
        // CreateValidator() -> ValidateForeignKey() -> ValidateAndExecuteAsync(ct) -> ValidateAsync(ct) -> validation(ct)
    }

    /// <summary>
    /// Example 2: Custom validation with CancellationToken support
    /// </summary>
    public static async Task<Result<object>> Example2_CustomValidationWithCancellationToken(
        IForeignKeyValidator fkValidator,
        IQueryContext queryContext,
        Guid userId,
        string email,
        CancellationToken cancellationToken)
    {
        return await fkValidator.CreateValidator()
            .ValidateForeignKey<User>(userId, "User")

            // Custom validation WITHOUT cancellation token (for simple sync checks)
            .AddCustomValidation(() =>
                string.IsNullOrWhiteSpace(email)
                    ? Result.Failure(ErrorType.Validation, "Email is required")
                    : Result.Success())

            // Custom validation WITH cancellation token (for async operations)
            .AddCustomValidation(async (ct) =>
            {
                var emailExists = await queryContext.Query<User>()
                    .AnyAsync(u => u.Email == email, ct); // CancellationToken được sử dụng!

                return emailExists
                    ? Result.Failure(ErrorType.Conflict, $"Email {email} already exists")
                    : Result.Success();
            })

            .ValidateAndExecuteAsync(async () =>
            {
                await Task.CompletedTask;
                return Result<object>.Success(new { Message = "All validations passed!" });
            }, cancellationToken);
    }

    /// <summary>
    /// Example 3: Multiple FK validations with arrays
    /// </summary>
    public static async Task<Result<object>> Example3_MultipleValidations(
        IForeignKeyValidator fkValidator,
        Guid[] assetIds,
        Guid[] locationIds,
        Guid? managerId, // nullable
        CancellationToken cancellationToken)
    {
        return await fkValidator.CreateValidator()
            // Validate multiple assets - CancellationToken passed internally
            .ValidateForeignKeys<Asset>(assetIds, "Asset")

            // Validate multiple locations
            .ValidateForeignKeys<Location>(locationIds, "Location")

            // Validate nullable FK - automatically skipped if null
            .ValidateForeignKey<User>(managerId, "Manager")

            .ValidateAndExecuteAsync(async () =>
            {
                await Task.CompletedTask;
                return Result<object>.Success(new { Message = "All FKs validated!" });
            }, cancellationToken);
    }

    /// <summary>
    /// Example 4: Complex scenario với nhiều loại validations
    /// </summary>
    public static async Task<Result<object>> Example4_ComplexScenario(
        IForeignKeyValidator fkValidator,
        IQueryContext queryContext,
        Guid userId,
        Guid organizationUnitId,
        Guid[] assetIds,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        return await fkValidator.CreateValidator()
            // Business rule validations (sync)
            .AddCustomValidation(() =>
                startDate >= endDate
                    ? Result.Failure(ErrorType.Validation, "Start date must be before end date")
                    : Result.Success())

            .AddCustomValidation(() =>
                assetIds.Length == 0
                    ? Result.Failure(ErrorType.Validation, "At least one asset is required")
                    : Result.Success())

            // FK validations
            .ValidateForeignKey<User>(userId, "User")
            .ValidateForeignKey<OrganizationUnit>(organizationUnitId, "Organization Unit")
            .ValidateForeignKeys<Asset>(assetIds, "Asset")

            // Complex async validation with DB query
            .AddCustomValidation(async (ct) =>
            {
                // Check if user belongs to organization unit
                var userBelongsToOrg = await queryContext.Query<User>()
                    .AnyAsync(u => u.Id == userId && u.OrganizationUnitId == organizationUnitId, ct);

                return userBelongsToOrg
                    ? Result.Success()
                    : Result.Failure(ErrorType.Validation, "User does not belong to the specified organization unit");
            })

            // Execute if all validations pass
            .ValidateAndExecuteAsync(async () =>
            {
                // All validations passed - safe to proceed
                await Task.CompletedTask;
                return Result<object>.Success(new
                {
                    Message = "All validations passed!",
                    UserId = userId,
                    OrganizationUnitId = organizationUnitId,
                    AssetCount = assetIds.Length
                });
            }, cancellationToken);
    }

    /// <summary>
    /// Example 5: Demonstrating cancellation
    /// </summary>
    public static async Task<Result<object>> Example5_CancellationDemo(
        IForeignKeyValidator fkValidator,
        CancellationToken cancellationToken)
    {
        // Nếu cancellationToken được cancel trước khi validation:
        // - ThrowIfCancellationRequested() sẽ throw OperationCanceledException
        // - Validation chain sẽ stop ngay lập tức
        // - Không waste resources cho validations tiếp theo

        return await fkValidator.CreateValidator()
            .ValidateForeignKey<User>(Guid.NewGuid(), "User") // Might be cancelled here
            .ValidateForeignKey<User>(Guid.NewGuid(), "User") // Or here
            .ValidateForeignKey<User>(Guid.NewGuid(), "User") // Or here
            .ValidateAndExecuteAsync(async () =>
            {
                await Task.CompletedTask;
                return Result<object>.Success(new { Message = "Completed" });
            }, cancellationToken);

        // Benefits:
        // 1. Respects cancellation throughout the entire validation chain
        // 2. Each validation can be cancelled independently
        // 3. Database queries in validations will also be cancelled
        // 4. Clean, responsive cancellation behavior
    }
}
