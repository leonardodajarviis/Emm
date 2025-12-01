namespace Emm.Domain.ValueObjects;

/// <summary>
/// Value Object - Kết quả validation
/// </summary>
public sealed class ValidationResult : IEquatable<ValidationResult>
{
    public bool IsValid { get; }
    public string? ErrorMessage { get; }

    private ValidationResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new(true);
    public static ValidationResult Failure(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("Error message is required for failure result", nameof(errorMessage));

        return new(false, errorMessage);
    }

    // Equality for Value Object
    public bool Equals(ValidationResult? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return IsValid == other.IsValid && ErrorMessage == other.ErrorMessage;
    }

    public override bool Equals(object? obj) => obj is ValidationResult other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(IsValid, ErrorMessage);

    public static bool operator ==(ValidationResult? left, ValidationResult? right) => Equals(left, right);
    public static bool operator !=(ValidationResult? left, ValidationResult? right) => !Equals(left, right);

    public override string ToString() => IsValid ? "Valid" : $"Invalid: {ErrorMessage}";
}
