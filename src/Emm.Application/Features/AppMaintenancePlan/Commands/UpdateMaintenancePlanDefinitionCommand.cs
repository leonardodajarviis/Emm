namespace Emm.Application.Features.AppMaintenancePlan.Commands;

public record UpdateMaintenancePlanDefinitionCommand : IRequest<Result>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }

    // For Time-based maintenance plans
    public string? RRule { get; init; }

    // For Parameter-based maintenance plans
    public decimal? Value { get; init; }
    public decimal? PlusTolerance { get; init; }
    public decimal? MinusTolerance { get; init; }
}
