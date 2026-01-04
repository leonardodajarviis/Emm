using Emm.Domain.Exceptions;

namespace Emm.Domain.ValueObjects;

public sealed class AssetStatus : IEquatable<AssetStatus>
{
    public static readonly AssetStatus Maintenance = new("maintenance");
    public static readonly AssetStatus Incident = new("incident");
    public static readonly AssetStatus Operating = new("operating");
    public static readonly AssetStatus Idle = new("idle");
    public string Value { get; }

    private AssetStatus(string value)
    {
        Value = value;
    }

    public override string ToString() => Value;

    public bool Equals(AssetStatus? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public static AssetStatus From(string value)
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
