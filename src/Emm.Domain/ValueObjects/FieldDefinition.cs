using Emm.Domain.Entities.Operations;

namespace Emm.Domain.ValueObjects;

/// <summary>
/// Value Object - Định nghĩa metadata của một field cho EAV pattern
/// Có thể tái sử dụng cho nhiều loại entity khác nhau
/// </summary>
public sealed class FieldDefinition : IEquatable<FieldDefinition>
{
    public string Name { get; }
    public FieldValueType FieldType { get; }
    public bool IsRequired { get; }
    public string? ValidationRulesJson { get; }

    public FieldDefinition(
        string name,
        FieldValueType fieldType,
        bool isRequired = false,
        string? validationRulesJson = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Field name is required", nameof(name));

        Name = name;
        FieldType = fieldType;
        IsRequired = isRequired;
        ValidationRulesJson = validationRulesJson;
    }

    /// <summary>
    /// Tạo FieldDefinition từ CheckpointTemplateField
    /// </summary>
    public static FieldDefinition FromCheckpointField(
        string name,
        FieldValueType fieldType,
        bool isRequired,
        string? validationRulesJson)
    {
        return new FieldDefinition(name, fieldType, isRequired, validationRulesJson);
    }

    // Equality for Value Object
    public bool Equals(FieldDefinition? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name
            && FieldType == other.FieldType
            && IsRequired == other.IsRequired
            && ValidationRulesJson == other.ValidationRulesJson;
    }

    public override bool Equals(object? obj) => obj is FieldDefinition other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Name, FieldType, IsRequired, ValidationRulesJson);

    public static bool operator ==(FieldDefinition? left, FieldDefinition? right) => Equals(left, right);
    public static bool operator !=(FieldDefinition? left, FieldDefinition? right) => !Equals(left, right);

    public override string ToString() => $"{Name} ({FieldType}){(IsRequired ? " [Required]" : "")}";
}
