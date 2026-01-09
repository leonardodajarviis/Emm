namespace Emm.Application.Features.AppOperationShift.Builder;

public class SyncShiftLogEventsHandler : IUpdateShiftLogBuilderHandler
{
    public SyncShiftLogEventsHandler()
    {
    }
    public async Task<Result> Handle(UpdateShiftLogContext context, CancellationToken cancellationToken)
    {
        var requestEventIds = context.Data.Events
            .Where(e => e.Id.HasValue)
            .Select(e => e.Id!.Value)
            .ToHashSet();

        var shiftLog = context.ShiftLog;

        CollectionHelper.RemoveItemsNotInRequest(
            context.ShiftLog.Events,
            requestEventIds,
            e => e.Id,
            context.ShiftLog.RemoveEvent);

        foreach (var ev in context.Data.Events)
        {
            if (ev.Id.HasValue)
            {
                shiftLog.UpdateEvent(
                    ev.Id.Value,
                    ev.EventType,
                    ev.StartTime,
                    ev.EndTime);
            }
            else
            {
                // New event, add it
                shiftLog.RecordEvent(
                    eventType: ev.EventType,
                    startTime: ev.StartTime,
                    endTime: ev.EndTime);
            }
        }

        return Result.Success();
    }
}
