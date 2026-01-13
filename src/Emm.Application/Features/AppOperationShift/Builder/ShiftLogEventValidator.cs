using Emm.Application.ErrorCodes;
using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Builder;

public static class ShiftLogEventValidator
{
    /// <summary>
    /// Validate that a list of events forms a valid timeline within the shift log's boundaries
    /// </summary>
    /// <param name="shiftLog">The shift log to validate against</param>
    /// <param name="events">List of events with StartTime and EndTime</param>
    /// <returns>Result indicating success or validation error</returns>
    public static Result ValidateEventsTimeline<T>(
        ShiftLog shiftLog,
        IEnumerable<T> events,
        Func<T, DateTime> getStartTime,
        Func<T, DateTime> getEndTime)
    {
        var eventList = events.ToList();

        foreach (var evt in eventList)
        {
            var startTime = getStartTime(evt);
            var endTime = getEndTime(evt);

            // Validate each event is within shift log boundaries
            var boundaryResult = ValidateEventWithinBoundaries(shiftLog, startTime, endTime);
            if (!boundaryResult.IsSuccess)
            {
                return boundaryResult;
            }
        }

        // Validate events don't overlap with each other
        var sortedEvents = eventList
            .Select(e => new { StartTime = getStartTime(e), EndTime = getEndTime(e) })
            .OrderBy(e => e.StartTime)
            .ToList();

        for (int i = 0; i < sortedEvents.Count - 1; i++)
        {
            var currentEvent = sortedEvents[i];
            var nextEvent = sortedEvents[i + 1];

            // Check if current event ends after next event starts (overlap)
            if (currentEvent.EndTime > nextEvent.StartTime)
            {
                return Result.Validation(
                    $"Các sự kiện bị chồng chéo: sự kiện kết thúc lúc {currentEvent.EndTime:yyyy-MM-dd HH:mm:ss} chồng với sự kiện bắt đầu lúc {nextEvent.StartTime:yyyy-MM-dd HH:mm:ss}.",
                    ShiftLogErrorCodes.EventTimeOutOfRange);
            }
        }

        return Result.Success();
    }

    /// <summary>
    /// Validate that an event's timeline is within the shift log's start and end time
    /// </summary>
    private static Result ValidateEventWithinBoundaries(
        ShiftLog shiftLog,
        DateTime eventStartTime,
        DateTime eventEndTime)
    {
        // Check if event start time is before shift log start time
        if (eventStartTime < shiftLog.StartTime)
        {
            return Result.Validation(
                $"Thời gian bắt đầu sự kiện ({eventStartTime:yyyy-MM-dd HH:mm:ss}) không thể trước thời gian bắt đầu của ca ({shiftLog.StartTime:yyyy-MM-dd HH:mm:ss}).",
                ShiftLogErrorCodes.EventTimeOutOfRange);
        }

        // Check if shift log has end time and event start time is after it
        if (shiftLog.EndTime.HasValue && eventStartTime > shiftLog.EndTime.Value)
        {
            return Result.Validation(
                $"Thời gian bắt đầu sự kiện ({eventStartTime:yyyy-MM-dd HH:mm:ss}) không thể sau thời gian kết thúc của ca ({shiftLog.EndTime.Value:yyyy-MM-dd HH:mm:ss}).",
                ShiftLogErrorCodes.EventTimeOutOfRange);
        }

        // Check if event end time is before shift log start time
        if (eventEndTime < shiftLog.StartTime)
        {
            return Result.Validation(
                $"Thời gian kết thúc sự kiện ({eventEndTime:yyyy-MM-dd HH:mm:ss}) không thể trước thời gian bắt đầu của ca ({shiftLog.StartTime:yyyy-MM-dd HH:mm:ss}).",
                ShiftLogErrorCodes.EventTimeOutOfRange);
        }

        // Check if shift log has end time and event end time is after it
        if (shiftLog.EndTime.HasValue && eventEndTime > shiftLog.EndTime.Value)
        {
            return Result.Validation(
                $"Thời gian kết thúc sự kiện ({eventEndTime:yyyy-MM-dd HH:mm:ss}) không thể sau thời gian kết thúc của ca ({shiftLog.EndTime.Value:yyyy-MM-dd HH:mm:ss}).",
                ShiftLogErrorCodes.EventTimeOutOfRange);
        }

        return Result.Success();
    }
}
