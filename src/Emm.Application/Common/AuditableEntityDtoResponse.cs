namespace Emm.Application.Common;

public record AuditableEntityDtoResponse
{
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public string? UpdatedBy { get; init; }
    public long? CreatedByUserId { get; init; }
    public long? UpdatedByUserId { get; init; }
}
