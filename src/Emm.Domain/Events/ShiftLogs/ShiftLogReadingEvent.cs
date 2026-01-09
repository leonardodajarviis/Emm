using Emm.Domain.Abstractions;
using Emm.Domain.Entities.Operations;

namespace Emm.Domain.Events.ShiftLogs;

public class ShiftLogReadingEvent: IDomainEvent
{
    public long ShiftLogId { get; }
    public IEnumerable<ShiftLogParameterReading> ParameterReadings { get; }

    public DateTime OccurredOn => DateTime.UtcNow;

    public ShiftLogReadingEvent(long shiftLogId, IEnumerable<ShiftLogParameterReading> parameterReadings)
    {
        ShiftLogId = shiftLogId;
        ParameterReadings = parameterReadings;
    }
}
