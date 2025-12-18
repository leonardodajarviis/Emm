namespace Emm.Domain.ValueObjects;

public record AuditMetadata
{
    public Guid CreatedByUserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid? ModifiedByUserId { get; init; }
    public DateTime? ModifiedAt { get; init; }

    public static AuditMetadata Create(Guid createdByUserId, DateTime at)
    {
        return new AuditMetadata
        {
            CreatedByUserId = createdByUserId,
            CreatedAt = at
        };
    }

    public AuditMetadata Update(Guid modifiedByUserId, DateTime at)
    {
        return this with
        {
            ModifiedByUserId = modifiedByUserId,
            ModifiedAt = at
        };
    }

    private AuditMetadata() { }
}
