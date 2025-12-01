using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Messaging;

public class OutboxMessage
{
    public Guid Id { get; set; }

    public string Type { get; set; } = null!;

    public string Payload { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public string? Error { get; set; }

    public int Attempt { get; set; }

    public string? LockId { get; set; }

    public DateTime? LockedUntil { get; set; }

    public byte[] RowVersion { get; set; } = default!;
}
