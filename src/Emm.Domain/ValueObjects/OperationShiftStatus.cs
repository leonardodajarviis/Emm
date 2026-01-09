using Emm.Domain.Exceptions;

namespace Emm.Domain.ValueObjects;

public sealed class OperationShiftStatus : IEquatable<OperationShiftStatus>
{
    public static readonly OperationShiftStatus Scheduled = new("scheduled");
    public static readonly OperationShiftStatus InProgress = new("in_progress");
    public static readonly OperationShiftStatus Completed = new("completed");
    public static readonly OperationShiftStatus Cancelled = new("cancelled");
    public static readonly OperationShiftStatus Paused = new("paused");
    public string Value { get; }

    private OperationShiftStatus(string value)
    {
        Value = value;
    }

    public override string ToString() => Value;

    public bool Equals(OperationShiftStatus? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public static OperationShiftStatus From(string value)
    {
        return value.ToLower() switch
        {
            "scheduled" => Scheduled,
            "in_progress" => InProgress,
            "completed" => Completed,
            "cancelled" => Cancelled,
            "paused" => Paused,
            _ => throw new DomainException($"Invalid OperationShiftStatus value: {value}")
        };
    }

}
