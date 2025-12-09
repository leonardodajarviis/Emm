namespace Emm.Application.Common;

public record AuditableEntityDtoResponse
{
    public required DateTime CreatedAt { get; init; }
    public required DateTime? ModifiedAt { get; init; }
    public string? CreatedBy { get; init; }
    public string? ModifiedBy { get; init; }
    public long? CreatedByUserId { get; init; }
    public long? ModifiedByUserId { get; init; }
}
