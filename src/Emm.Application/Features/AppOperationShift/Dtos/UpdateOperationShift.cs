namespace Emm.Application.Features.AppOperationShift.Dtos;

public record UpdateOperationShift
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required long LocationId { get; set; }
    public required long PrimaryOperatorId { get; set; }
    public required string PrimaryOperatorName { get; set; }
    public required DateTime ScheduledStartTime { get; set; }
    public required DateTime ScheduledEndTime { get; set; }
    public string? Notes { get; set; }
}
