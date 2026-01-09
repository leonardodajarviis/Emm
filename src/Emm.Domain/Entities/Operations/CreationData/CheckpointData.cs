namespace Emm.Domain.Entities.Operations.CreationData;

public sealed record CheckpointData
{
    public Guid LinkedId { get; init; }
    public string Name { get; init; } = null!;
    public Guid LocationId { get; init; }
    public required string LocationName { get; init; }
    public bool IsWithAttachedMaterial { get; init; }
    public Guid? ItemId { get; init; }
    public string? ItemCode {get; init;}
    public string? ItemName { get; init; }
}
