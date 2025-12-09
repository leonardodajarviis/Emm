using Emm.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Emm.Infrastructure.Data.Converters;

public class NaturalKeyConverter : ValueConverter<NaturalKey, string>
{
    public NaturalKeyConverter()
        : base(
            k => k.Value,
            s => NaturalKey.Parse(s)
        )
    { }
}
