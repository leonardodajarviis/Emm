namespace Emm.Domain.Entities.Operations.CreationData;

public sealed record EventData
{
    public ShiftLogEventType EventType { get; init; }
    public DateTime StartTime { get; init; }
}
