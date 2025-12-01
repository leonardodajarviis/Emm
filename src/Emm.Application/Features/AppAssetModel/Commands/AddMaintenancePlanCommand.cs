using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetModel.Commands;

public record AddMaintenancePlanCommand(
    long AssetModelId,
    string Name,
    string? Description,
    bool IsActive,
    MaintenancePlanType PlanType,

    // For Time-based maintenance plans
    string? RRule,

    // For Parameter-based maintenance plans
    long? ParameterId,
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
    long? OrganizationUnitId,
    string? Note,
    int Order
);

public record AddMaintenancePlanRequiredItemCommand(
    long ItemId,
    decimal Quantity,
    bool IsRequired,
    string? Note
);
