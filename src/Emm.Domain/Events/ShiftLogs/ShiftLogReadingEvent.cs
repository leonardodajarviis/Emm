using Emm.Domain.Abstractions;
using Emm.Domain.Entities.Operations;

namespace Emm.Domain.Events.ShiftLogs;

public class ShiftLogReadingEvent: IDomainEvent
{
    public Guid ShiftLogId { get; }
    public IEnumerable<ShiftLogParameterReading> ParameterReadings { get; }

    public DateTime OccurredOn => DateTime.UtcNow;

    public ShiftLogReadingEvent(Guid shiftLogId, IEnumerable<ShiftLogParameterReading> parameterReadings)
    {
        ShiftLogId = shiftLogId;
        ParameterReadings = parameterReadings;
    }
}
