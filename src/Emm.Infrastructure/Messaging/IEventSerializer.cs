using System.Text.Json;
using Emm.Domain.Abstractions;

namespace Emm.Infrastructure.Messaging;

public interface IEventSerializer
{
    string Serialize(IDomainEvent e);
    IDomainEvent Deserialize(string type, string payload);
}

public sealed class SystemTextJsonEventSerializer : IEventSerializer
{
    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        WriteIndented = false
        // Note: Consider using JsonPolymorphism/JsonDerivedType for polymorphic event serialization
    };

    public string Serialize(IDomainEvent e)
        => JsonSerializer.Serialize(e, e.GetType(), _jsonOpts);

    public IDomainEvent Deserialize(string type, string payload)
    {
        var t = Type.GetType(type, throwOnError: true)!;
        return (IDomainEvent)JsonSerializer.Deserialize(payload, t, _jsonOpts)!;
    }
}
