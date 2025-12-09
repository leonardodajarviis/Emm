using System.Text.RegularExpressions;
using Emm.Domain.Exceptions;

namespace Emm.Domain.ValueObjects;

/// <summary>
/// Value Object representing an email address
/// Immutable and validates email format
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private const int MaxLength = 320; // RFC 5321
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    private static readonly Regex _emailRegex = new(EmailPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email cannot be empty");

        value = value.Trim().ToLowerInvariant();

        if (value.Length > MaxLength)
            throw new DomainException($"Email cannot exceed {MaxLength} characters");

        if (!_emailRegex.IsMatch(value))
            throw new DomainException($"Invalid email format: {value}");

        // Additional validation rules
        if (value.Contains(".."))
            throw new DomainException("Email cannot contain consecutive dots");

        if (value.StartsWith('.') || value.EndsWith('.'))
            throw new DomainException("Email cannot start or end with a dot");

        Value = value;
    }

    public string Domain
    {
        get
        {
            var atIndex = Value.IndexOf('@');
            return atIndex >= 0 ? Value[(atIndex + 1)..] : string.Empty;
        }
    }

    public string LocalPart
    {
        get
        {
            var atIndex = Value.IndexOf('@');
            return atIndex >= 0 ? Value[..atIndex] : string.Empty;
        }
    }

    public bool IsFromDomain(string domain)
    {
        if (string.IsNullOrWhiteSpace(domain))
            return false;

        return Domain.Equals(domain.Trim().ToLowerInvariant(), StringComparison.OrdinalIgnoreCase);
    }

    // Equality for Value Object
    public bool Equals(Email? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is Email other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Email? left, Email? right) => Equals(left, right);
    public static bool operator !=(Email? left, Email? right) => !Equals(left, right);

    public override string ToString() => Value;

    // Implicit conversion to string for convenience
    public static implicit operator string(Email email) => email.Value;

    // Explicit conversion from string
    public static explicit operator Email(string value) => new(value);
}
