using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetModel.Commands;

public record CreateAssetModelCommand(
    bool IsCodeGenerated,
    string Code,
    string Name,
    string? Description,
    string? Notes,
    Guid? ParentId,
    Guid AssetCategoryId,
    Guid AssetTypeId,
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
    Guid? ParameterId,
    decimal? TriggerValue,
    decimal? PlusTolerance,
    decimal? MinusTolerance,
    MaintenanceTriggerCondition? TriggerCondition,
    // Job steps (common for both types)
    IReadOnlyCollection<MaintenancePlanJobStepDefinitionCommand>? JobSteps,
    IReadOnlyCollection<MaintenancePlanRequiredItemDefinitionCommand>? RequiredItems
);

public sealed record MaintenancePlanJobStepDefinitionCommand(
    Guid? Id,
    string Name,
    Guid? OrganizationUnitId,
    string? Note,
    int Order
);

public sealed record MaintenancePlanRequiredItemDefinitionCommand(
    Guid? Id,
    Guid ItemGroupId,
    Guid ItemId,
    Guid UnitOfMeasureId,
    decimal Quantity,
    bool IsRequired,
    string? Note
);

public sealed record CreateAssetModelImageCommand(
    Guid FileId
);
