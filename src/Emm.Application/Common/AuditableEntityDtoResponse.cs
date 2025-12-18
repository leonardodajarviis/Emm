namespace Emm.Application.Common;

public record AuditableEntityDtoResponse
{
    public required DateTime CreatedAt { get; init; }
    public required DateTime? ModifiedAt { get; init; }
    public string? CreatedBy { get; init; }
    public string? ModifiedBy { get; init; }
    public Guid? CreatedByUserId { get; init; }
    public Guid? ModifiedByUserId { get; init; }
}
