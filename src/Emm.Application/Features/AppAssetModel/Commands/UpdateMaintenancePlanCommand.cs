using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetModel.Commands;

public record UpdateMaintenancePlanCommand(
    Guid AssetModelId,
    Guid MaintenancePlanId,
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
    Guid? Id,  // null = new, has value = update
    string Name,
    Guid? OrganizationUnitId,
    string? Note,
    int Order
);

public record UpdateRequiredItemCommand(
    Guid? Id,  // null = new, has value = update
    Guid ItemId,
    decimal Quantity,
    bool IsRequired,
    string? Note
);
