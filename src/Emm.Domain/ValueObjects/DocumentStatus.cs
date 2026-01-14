using Emm.Domain.Exceptions;

namespace Emm.Domain.ValueObjects;

public sealed class DocumentStatus : IEquatable<DocumentStatus>
{
    public static readonly DocumentStatus Maintenance = new("maintenance");
    public static readonly DocumentStatus Incident = new("incident");
    public static readonly DocumentStatus Operating = new("operating");
    public static readonly DocumentStatus Idle = new("idle");
    public string Value { get; }

    private DocumentStatus(string value)
    {
        Value = value;
    }

    public override string ToString() => Value;

    public bool Equals(DocumentStatus? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public static DocumentStatus From(string value)
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
