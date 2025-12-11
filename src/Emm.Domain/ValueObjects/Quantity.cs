using Emm.Domain.Exceptions;

namespace Emm.Domain.ValueObjects;

/// <summary>
/// Value Object representing a quantity with unit of measure symbol
/// Immutable and contains business logic for quantity operations
/// Note: Unit is stored as symbol (e.g., "kg", "l", "pcs") for display purposes
/// The actual UnitOfMeasure entity should be validated at the application layer
/// </summary>
public sealed class Quantity : IEquatable<Quantity>, IComparable<Quantity>
{
    public decimal Value { get; }
    public Quantity(decimal value)
    {
        if (value < 0)
            throw new DomainException("Quantity value cannot be negative");

        Value = value;
    }

    public static Quantity Zero() => new(0);

    public Quantity Add(Quantity other)
    {
        return new Quantity(Value + other.Value);
    }

    public Quantity Subtract(Quantity other)
    {
        if (Value < other.Value)
            throw new DomainException("Cannot subtract to negative quantity");

        return new Quantity(Value - other.Value);
    }

    public Quantity Multiply(decimal factor)
    {
        if (factor < 0)
            throw new DomainException("Cannot multiply quantity by negative factor");

        return new Quantity(Value * factor);
    }

    public Quantity Divide(decimal divisor)
    {
        if (divisor <= 0)
            throw new DomainException("Cannot divide quantity by zero or negative number");

        return new Quantity(Value / divisor);
    }

    public bool IsZero => Value == 0;

    public bool IsPositive => Value > 0;

    // Equality for Value Object
    public bool Equals(Quantity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is Quantity other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Value);

    public int CompareTo(Quantity? other)
    {
        if (other is null) return 1;

        return Value.CompareTo(other.Value);
    }

    public static bool operator ==(Quantity? left, Quantity? right) => Equals(left, right);
    public static bool operator !=(Quantity? left, Quantity? right) => !Equals(left, right);
    public static bool operator >(Quantity left, Quantity right) => left.CompareTo(right) > 0;
    public static bool operator <(Quantity left, Quantity right) => left.CompareTo(right) < 0;
    public static bool operator >=(Quantity left, Quantity right) => left.CompareTo(right) >= 0;
    public static bool operator <=(Quantity left, Quantity right) => left.CompareTo(right) <= 0;

    public static Quantity operator +(Quantity left, Quantity right) => left.Add(right);
    public static Quantity operator -(Quantity left, Quantity right) => left.Subtract(right);
    public static Quantity operator *(Quantity quantity, decimal factor) => quantity.Multiply(factor);
    public static Quantity operator *(decimal factor, Quantity quantity) => quantity.Multiply(factor);
    public static Quantity operator /(Quantity quantity, decimal divisor) => quantity.Divide(divisor);

    public override string ToString() => Value.ToString();
}
