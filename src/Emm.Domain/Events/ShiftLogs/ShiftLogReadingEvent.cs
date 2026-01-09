using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.ShiftLogs;

public class ShiftLogReadingEvent: IDeferredDomainEvent
{
    public Guid ShiftLogId { get; }
    public IEnumerable<ShiftLogParameterReadingEventData> ParameterReadings { get; }

    public DateTime OccurredOn => DateTime.UtcNow;

    public ShiftLogReadingEvent(Guid shiftLogId, IEnumerable<ShiftLogParameterReadingEventData> parameterReadings)
    {
        ShiftLogId = shiftLogId;
        ParameterReadings = parameterReadings;
    }
}

public record ShiftLogParameterReadingEventData
{
    public Guid AssetId { get; init; }
    public Guid ParameterId { get; init; }
    public decimal Value { get; init; }
}
