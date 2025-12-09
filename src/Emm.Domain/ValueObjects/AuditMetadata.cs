namespace Emm.Domain.ValueObjects;

public record AuditMetadata
{
    public long CreatedByUserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public long? ModifiedByUserId { get; init; }
    public DateTime? ModifiedAt { get; init; }

    public static AuditMetadata Create(long createdByUserId, DateTime at)
    {
        return new AuditMetadata
        {
            CreatedByUserId = createdByUserId,
            CreatedAt = at
        };
    }

    public AuditMetadata Update(long modifiedByUserId, DateTime at)
    {
        return this with
        {
            ModifiedByUserId = modifiedByUserId,
            ModifiedAt = at
        };
    }

    private AuditMetadata() { }
}
