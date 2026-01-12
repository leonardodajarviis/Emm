using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppMaintenancePlan.Commands;

// Base command for creating maintenance plan (common properties)
public record CreateMaintenancePlanDefinitionCommand : IRequest<Result>
{
    public required Guid AssetModelId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required MaintenancePlanType PlanType { get; init; }
    public bool IsActive { get; init; } = true;

    // For Time-based maintenance plans
    public string? RRule { get; init; }

    // For Parameter-based maintenance plans
    public Guid? ParameterId { get; init; }
    public decimal? Value { get; init; }
    public decimal? PlusTolerance { get; init; }
    public decimal? MinusTolerance { get; init; }

    // Job steps (common for both types)
    public IReadOnlyCollection<CreateJobStepCommand>? JobSteps { get; init; }

    // Required items (common for both types)
    public IReadOnlyCollection<CreateRequiredItemCommand>? RequiredItems { get; init; }
}

public record CreateJobStepCommand
{
    public required string Name { get; init; }
    public Guid? OrganizationUnitId { get; init; }
    public string? Note { get; init; }
    public int Order { get; init; }
}

public record CreateRequiredItemCommand
{
    public required Guid ItemGroupId { get; init; }
    public required Guid ItemId { get; init; }
    public required Guid UnitOfMeasureId { get; init; }
    public required decimal Quantity { get; init; }
    public required bool IsRequired { get; init; }
    public string? Note { get; init; }
}
