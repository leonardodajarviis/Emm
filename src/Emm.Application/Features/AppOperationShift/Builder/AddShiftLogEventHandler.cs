namespace Emm.Application.Features.AppOperationShift.Builder;

public class AddShiftLogEventHandler : ICreateShiftLogBuilderHandler
{
    public Task<Result> Handle(CreateShiftLogContext context, CancellationToken cancellationToken)
    {
        var shiftLog = context.ShiftLog;
        var data = context.Data;

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
