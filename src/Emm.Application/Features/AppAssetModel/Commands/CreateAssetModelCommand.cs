using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetModel.Commands;

public record CreateAssetModelCommand(
    string Name,
    string? Description,
    string? Notes,
    long? ParentId,
    long? AssetCategoryId,
    long? AssetTypeId,
    IReadOnlyCollection<long>? ParameterIds,
    IReadOnlyCollection<CreateMaintenancePlanDefinitionCommand>? MaintenancePlanDefinitions,
    IReadOnlyCollection<CreateAssetModelImageCommand>? Images,
    Guid? ThumbnailFileId,
    bool IsActive = true
) : IRequest<Result<object>>;

public sealed record CreateMaintenancePlanDefinitionCommand(
    string Name,
    string? Description,
    MaintenancePlanType PlanType,
    bool IsActive,

    // For Time-based maintenance plans
    string? RRule,

    // For Parameter-based maintenance plans
    long? ParameterId,
    decimal? TriggerValue,
    decimal? MinValue,
    decimal? MaxValue,
    MaintenanceTriggerCondition? TriggerCondition,
    // Job steps (common for both types)
    IReadOnlyCollection<MaintenancePlanJobStepDefinitionCommand>? JobSteps
);

public sealed record MaintenancePlanJobStepDefinitionCommand(
    string Name,
    long? OrganizationUnitId,
    string? Note,
    int Order
);

public sealed record CreateAssetModelImageCommand(
    Guid FileId
);
