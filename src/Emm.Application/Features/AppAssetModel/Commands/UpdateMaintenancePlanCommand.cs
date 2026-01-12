using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetModel.Commands;

public record UpdateMaintenancePlanCommand(
    Guid AssetModelId,
    Guid MaintenancePlanId,
    UpdateMaintenancePlanBody Body
) : IRequest<Result<object>>;

public record UpdateMaintenancePlanBody(
    string Name,
    string? Description,
    bool IsActive,
    MaintenancePlanType PlanType,

    // For Time-based maintenance plans
    string? RRule,

    // For Parameter-based maintenance plans
    decimal? TriggerValue,
    decimal? MinusTolerance,
    decimal? PlusTolerance,
    MaintenanceTriggerCondition? TriggerCondition,

    // Job steps (common for both types)
    IReadOnlyCollection<MaintenancePlanJobStepDefinitionCommand>? JobSteps,

    // Required items (vật tư phụ tùng)
    IReadOnlyCollection<MaintenancePlanRequiredItemDefinitionCommand>? RequiredItems
);
