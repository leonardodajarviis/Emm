using System.Text.Json.Serialization;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record UpdateShiftLogCommand : IRequest<Result>
{
    public long OperationShiftId { get; set; }

    [JsonIgnore]
    public long ShiftLogId { get; set; }

    public string? Name { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime StartTime { get; set; }
    public IEnumerable<ParameterReadingRequest>? Readings { get; set; }
    public IEnumerable<CheckpointRequest>? Checkpoints { get; set; }
    public IEnumerable<LogEventRequest>? Events { get; set; }
    public IEnumerable<ShiftLogItemRequest>? Items { get; set; }
}
