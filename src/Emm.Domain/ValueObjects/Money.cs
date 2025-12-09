using Emm.Domain.Exceptions;

namespace Emm.Domain.ValueObjects;

/// <summary>
/// Value Object representing a monetary amount with currency
/// Immutable and contains business logic for money operations
/// </summary>
public sealed class Money : IEquatable<Money>, IComparable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    private static readonly HashSet<string> _supportedCurrencies =
    [
        "VND", "USD", "EUR", "JPY", "CNY", "KRW", "THB", "SGD"
    ];

    public Money(decimal amount, string currency = "VND")
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency cannot be empty");

        if (currency.Length != 3)
            throw new DomainException("Currency code must be 3 characters (ISO 4217)");

        var upperCurrency = currency.ToUpperInvariant();
        if (!_supportedCurrencies.Contains(upperCurrency))
            throw new DomainException($"Currency '{currency}' is not supported");

        if (amount < 0)
            throw new DomainException("Money amount cannot be negative");

        Amount = amount;
        Currency = upperCurrency;
    }

    public static Money Zero(string currency = "VND") => new(0, currency);

    public static Money FromVND(decimal amount) => new(amount, "VND");

    public static Money FromUSD(decimal amount) => new(amount, "USD");

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot add money with different currencies: {Currency} and {other.Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot subtract money with different currencies: {Currency} and {other.Currency}");

        if (Amount < other.Amount)
            throw new DomainException("Cannot subtract to negative amount");

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {
        if (factor < 0)
            throw new DomainException("Cannot multiply money by negative factor");

        return new Money(Amount * factor, Currency);
    }

    public Money Divide(decimal divisor)
    {
        if (divisor <= 0)
            throw new DomainException("Cannot divide money by zero or negative number");

        return new Money(Amount / divisor, Currency);
    }

    public bool IsZero => Amount == 0;

    public bool IsPositive => Amount > 0;

    // Equality for Value Object
    public bool Equals(Money? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj) => obj is Money other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    public int CompareTo(Money? other)
    {
        if (other is null) return 1;
        if (Currency != other.Currency)
            throw new DomainException($"Cannot compare money with different currencies: {Currency} and {other.Currency}");

        return Amount.CompareTo(other.Amount);
    }

    public static bool operator ==(Money? left, Money? right) => Equals(left, right);
    public static bool operator !=(Money? left, Money? right) => !Equals(left, right);
    public static bool operator >(Money left, Money right) => left.CompareTo(right) > 0;
    public static bool operator <(Money left, Money right) => left.CompareTo(right) < 0;
    public static bool operator >=(Money left, Money right) => left.CompareTo(right) >= 0;
    public static bool operator <=(Money left, Money right) => left.CompareTo(right) <= 0;

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money money, decimal factor) => money.Multiply(factor);
    public static Money operator *(decimal factor, Money money) => money.Multiply(factor);
    public static Money operator /(Money money, decimal divisor) => money.Divide(divisor);

    public override string ToString() => $"{Amount:N0} {Currency}";

    public string ToString(string format) => $"{Amount.ToString(format)} {Currency}";
}
