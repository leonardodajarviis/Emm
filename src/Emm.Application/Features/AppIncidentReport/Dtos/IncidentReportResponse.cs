using Emm.Domain.Entities.Maintenance;

namespace Emm.Application.Features.AppIncidentReport.Dtos;

public record IncidentReportResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required long AssetId { get; set; }
    public string? AssetName { get; set; }
    public required long CreatedById { get; set; }
    public string? CreatedBy { get; set; }
    public required DateTime ReportedAt { get; set; }
    public required IncidentPriority Priority { get; set; }
    public required IncidentStatus Status { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolutionNotes { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}