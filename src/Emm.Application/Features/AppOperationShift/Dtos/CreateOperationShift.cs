namespace Emm.Application.Features.AppOperationShift.Dtos;

public record CreateOperationShift
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required long LocationId { get; set; }
    public required long OrganizationUnitId { get; set; }
    public required DateTime ScheduledStartTime { get; set; }
    public required DateTime ScheduledEndTime { get; set; }
    public string? Notes { get; set; }

    public IReadOnlyCollection<long> AssetIds { get; set; } = [];
    // Asset assignments
}
