using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetModel.Commands;

public record AddMaintenancePlanCommand(
    Guid AssetModelId,
    AddMaintenancePlanCommandBody Body
) : IRequest<Result<object>>;

public record AddMaintenancePlanCommandBody(
    string Name,
    string? Description,
    bool IsActive,
    MaintenancePlanType PlanType,

    // For Time-based maintenance plans
    string? RRule,

    // For Parameter-based maintenance plans
    Guid? ParameterId,
    decimal? TriggerValue,
    decimal? MinValue,
    decimal? MaxValue,
    MaintenanceTriggerCondition? TriggerCondition,

    // Job steps (common for both types)
    IReadOnlyCollection<MaintenancePlanJobStepDefinitionCommand>? JobSteps,

    // Required items (vật tư phụ tùng)
    IReadOnlyCollection<MaintenancePlanRequiredItemDefinitionCommand>? RequiredItems
);
