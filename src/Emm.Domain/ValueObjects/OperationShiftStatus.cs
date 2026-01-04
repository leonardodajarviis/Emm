using Emm.Domain.Exceptions;

namespace Emm.Domain.ValueObjects;

public sealed class OperationShiftStatus : IEquatable<AssetStatus>
{
    public static readonly OperationShiftStatus Maintenance = new("maintenance");
    public static readonly OperationShiftStatus Incident = new("incident");
    public static readonly OperationShiftStatus Operating = new("operating");
    public static readonly OperationShiftStatus Idle = new("idle");
    public string Value { get; }

    private OperationShiftStatus(string value)
    {
        Value = value;
    }

    public override string ToString() => Value;

    public bool Equals(AssetStatus? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public static OperationShiftStatus From(string value)
    {
        return value.ToLowerInvariant() switch
        {
            "maintenance" => Maintenance,
            "incident" => Incident,
            "operating" => Operating,
            "idle" => Idle,
            _ => throw new DomainException($"Invalid asset status: {value}")
        };
    }

}
