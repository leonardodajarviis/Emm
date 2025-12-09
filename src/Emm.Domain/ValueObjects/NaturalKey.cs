using System.Text.RegularExpressions;
using Emm.Domain.Exceptions;

namespace Emm.Domain.ValueObjects;

public readonly record struct NaturalKey : IEquatable<NaturalKey>
{
    public string Value { get; }
    public string Prefix { get; }
    public int Number { get; }
    public int Padding { get; }

    private NaturalKey(string prefix, int number, int padding)
    {
        Prefix = prefix;
        Number = number;
        Padding = padding;
        Value = $"{prefix}-{number.ToString().PadLeft(padding, '0')}";
    }

    public static NaturalKey Create(string prefix, int number, int padding)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new DomainException("Prefix cannot be empty");

        prefix = prefix.Trim().ToUpperInvariant();

        if (!Regex.IsMatch(prefix, "^[A-Z]+$"))
        {
            throw new DomainException("Prefix must contain only uppercase letters A-Z");

        }

        if (number < 0)
        {
            throw new DomainException("Number cannot be negative");
        }

        return new NaturalKey(prefix, number, padding);
    }

    public static NaturalKey Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Value cannot be empty");

        var parts = value.Split('-');
        if (parts.Length != 2)
            throw new DomainException("Invalid NaturalKey format");

        var prefix = parts[0];
        var numberPart = parts[1];

        if (!int.TryParse(numberPart, out int number))
            throw new DomainException("Invalid number part in NaturalKey");

        var padding = numberPart.Length;

        return Create(prefix, number, padding);
    }

    public override string ToString() => Value;
    public override int GetHashCode() => Value.GetHashCode();
}
