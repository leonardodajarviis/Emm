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
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = false
        // Gợi ý: dùng JsonPolymorphism/JsonDerivedType cho các event cụ thể
    };

    public string Serialize(IDomainEvent e)
        => JsonSerializer.Serialize(e, e.GetType(), JsonOpts);

    public IDomainEvent Deserialize(string type, string payload)
    {
        var t = Type.GetType(type, throwOnError: true)!;
        return (IDomainEvent)JsonSerializer.Deserialize(payload, t, JsonOpts)!;
    }
}
