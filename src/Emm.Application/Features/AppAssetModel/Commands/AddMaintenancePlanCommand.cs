using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetModel.Commands;

public record AddMaintenancePlanCommand(
    Guid AssetModelId,
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
    IReadOnlyCollection<AddMaintenancePlanJobStepCommand>? JobSteps,

    // Required items (vật tư phụ tùng)
    IReadOnlyCollection<AddMaintenancePlanRequiredItemCommand>? RequiredItems
) : IRequest<Result<object>>;

public record AddMaintenancePlanJobStepCommand(
    string Name,
    Guid? OrganizationUnitId,
    string? Note,
    int Order
);

public record AddMaintenancePlanRequiredItemCommand(
    Guid ItemId,
    decimal Quantity,
    bool IsRequired,
    string? Note
);
