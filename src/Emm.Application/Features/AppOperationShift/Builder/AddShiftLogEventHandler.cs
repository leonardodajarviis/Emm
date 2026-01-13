namespace Emm.Application.Features.AppOperationShift.Builder;

public class AddShiftLogEventHandler : ICreateShiftLogBuilderHandler
{
    public Task<Result> Handle(CreateShiftLogContext context, CancellationToken cancellationToken)
    {
        var shiftLog = context.ShiftLog;
        var data = context.Data;

        // Validate all events form a valid timeline
        var validationResult = ShiftLogEventValidator.ValidateEventsTimeline(
            shiftLog,
            data.Events,
            e => e.StartTime,
            e => e.EndTime);

        if (!validationResult.IsSuccess)
        {
            return Task.FromResult(validationResult);
        }

        // Add all events
        foreach (var logEvent in data.Events)
        {
            shiftLog.RecordEvent(
                eventType: logEvent.EventType,
                startTime: logEvent.StartTime,
                endTime: logEvent.EndTime
            );
        }

        return Task.FromResult(Result.Success());
    }
}
