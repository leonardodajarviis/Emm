using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetModel.Commands;

public record UpdateMaintenancePlanCommand(
    long AssetModelId,
    long MaintenancePlanId,
    string Name,
    string? Description,
    bool IsActive,
    MaintenancePlanType PlanType,
    // For Time-based plans
    string? RRule,
    // For Parameter-based plans
    decimal? TriggerValue,
    decimal? MinValue,
    decimal? MaxValue,
    MaintenanceTriggerCondition? TriggerCondition,
    // Job steps
    IReadOnlyCollection<UpdateJobStepCommand>? JobSteps,
    // Required items (vật tư phụ tùng)
    IReadOnlyCollection<UpdateRequiredItemCommand>? RequiredItems
) : IRequest<Result<object>>;

public record UpdateJobStepCommand(
    long? Id,  // null = new, has value = update
    string Name,
    long? OrganizationUnitId,
    string? Note,
    int Order
);

public record UpdateRequiredItemCommand(
    long? Id,  // null = new, has value = update
    long ItemId,
    decimal Quantity,
    bool IsRequired,
    string? Note
);
