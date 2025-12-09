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
    public string Unit { get; }

    public Quantity(decimal value, string unit)
    {
        if (value < 0)
            throw new DomainException("Quantity value cannot be negative");

        if (string.IsNullOrWhiteSpace(unit))
            throw new DomainException("Unit of measure cannot be empty");

        if (unit.Length > 20)
            throw new DomainException("Unit of measure cannot exceed 20 characters");

        Value = value;
        Unit = unit.Trim().ToLowerInvariant();
    }

    public static Quantity Zero(string unit) => new(0, unit);

    public static Quantity FromPieces(decimal value) => new(value, "pcs");

    public static Quantity FromKilograms(decimal value) => new(value, "kg");

    public static Quantity FromLiters(decimal value) => new(value, "l");

    public static Quantity FromMeters(decimal value) => new(value, "m");

    public static Quantity FromHours(decimal value) => new(value, "h");

    public Quantity Add(Quantity other)
    {
        if (!IsSameUnit(other))
            throw new DomainException($"Cannot add quantities with different units: {Unit} and {other.Unit}");

        return new Quantity(Value + other.Value, Unit);
    }

    public Quantity Subtract(Quantity other)
    {
        if (!IsSameUnit(other))
            throw new DomainException($"Cannot subtract quantities with different units: {Unit} and {other.Unit}");

        if (Value < other.Value)
            throw new DomainException("Cannot subtract to negative quantity");

        return new Quantity(Value - other.Value, Unit);
    }

    public Quantity Multiply(decimal factor)
    {
        if (factor < 0)
            throw new DomainException("Cannot multiply quantity by negative factor");

        return new Quantity(Value * factor, Unit);
    }

    public Quantity Divide(decimal divisor)
    {
        if (divisor <= 0)
            throw new DomainException("Cannot divide quantity by zero or negative number");

        return new Quantity(Value / divisor, Unit);
    }

    public bool IsSameUnit(Quantity other)
    {
        if (other is null) return false;
        return Unit.Equals(other.Unit, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsZero => Value == 0;

    public bool IsPositive => Value > 0;

    // Equality for Value Object
    public bool Equals(Quantity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value && Unit.Equals(other.Unit, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => obj is Quantity other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Value, Unit.ToLowerInvariant());

    public int CompareTo(Quantity? other)
    {
        if (other is null) return 1;
        if (!IsSameUnit(other))
            throw new DomainException($"Cannot compare quantities with different units: {Unit} and {other.Unit}");

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

    public override string ToString() => $"{Value:N2} {Unit}";

    public string ToString(string format) => $"{Value.ToString(format)} {Unit}";
}
