using Emm.Domain.Exceptions;
using Emm.Domain.Services;
using Emm.Domain.ValueObjects;
using System.Text.Json;

namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Field động trong CheckpointTemplate
/// </summary>
public class CheckpointTemplateField
{
    public long Id { get; private set; }
    public long CheckpointTemplateId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public FieldValueType FieldType { get; private set; }
    public bool IsRequired { get; private set; }
    public bool IsActive { get; private set; }
    public int Order { get; private set; }
    public string? DefaultValue { get; private set; }

    /// <summary>
    /// JSON chứa các validation rules
    /// Ví dụ: {"min": 0, "max": 100, "regex": "^[A-Z]+$", "minLength": 5}
    /// </summary>
    public string? ValidationRules { get; private set; }

    /// <summary>
    /// FK đến MasterData nếu FieldType là MasterDataReference
    /// Ví dụ: ID của bảng Department, Location, Equipment, etc.
    /// </summary>
    public long? MasterDataTypeId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public CheckpointTemplateField(
        long checkpointTemplateId,
        string code,
        string name,
        string description,
        FieldValueType fieldType,
        bool isRequired = false,
        int order = 0,
        string? defaultValue = null,
        string? validationRules = null,
        long? masterDataTypeId = null,
        IFieldValidationService? validationService = null)
    {
        if (checkpointTemplateId <= 0)
            throw new DomainException("Invalid checkpoint template ID");

        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Field code is required");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Field name is required");

        // Validate: nếu là MasterDataReference thì phải có MasterDataTypeId
        if (fieldType == FieldValueType.MasterDataReference && !masterDataTypeId.HasValue)
            throw new DomainException("MasterDataTypeId is required for MasterDataReference field type");

        // Validate JSON format nếu có ValidationRules
        ValidateRulesJsonFormat(validationRules, validationService);

        CheckpointTemplateId = checkpointTemplateId;
        Code = code;
        Name = name;
        Description = description;
        FieldType = fieldType;
        IsRequired = isRequired;
        IsActive = true;
        Order = order;
        DefaultValue = defaultValue;
        ValidationRules = validationRules;
        MasterDataTypeId = masterDataTypeId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(
        string name,
        string description,
        bool isRequired,
        string? defaultValue = null,
        string? validationRules = null,
        IFieldValidationService? validationService = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Field name is required");

        // Validate JSON format nếu có ValidationRules
        ValidateRulesJsonFormat(validationRules, validationService);

        Name = name;
        Description = description;
        IsRequired = isRequired;
        DefaultValue = defaultValue;
        ValidationRules = validationRules;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Helper method để validate JSON format của ValidationRules
    /// Sử dụng service nếu có, fallback về JsonDocument.Parse
    /// </summary>
    private static void ValidateRulesJsonFormat(string? validationRules, IFieldValidationService? validationService)
    {
        if (string.IsNullOrWhiteSpace(validationRules))
            return;

        if (validationService != null)
        {
            var result = validationService.ValidateRulesJson(validationRules);
            if (!result.IsValid)
                throw new DomainException(result.ErrorMessage!);
        }
        else
        {
            // Fallback: validate JSON format bằng JsonDocument
            try
            {
                JsonDocument.Parse(validationRules);
            }
            catch
            {
                throw new DomainException("ValidationRules must be valid JSON");
            }
        }
    }

    public void UpdateOrder(int newOrder)
    {
        if (newOrder < 0)
            throw new DomainException("Order must be non-negative");

        Order = newOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Validate giá trị theo ValidationRules và FieldType
    /// Delegate tới FieldValidationService để có thể tái sử dụng logic này cho các EAV khác
    /// </summary>
    public ValidationResult ValidateValue(string? value, IFieldValidationService validationService)
    {
        return validationService.ValidateValue(value, Name, FieldType, IsRequired, ValidationRules);
    }

    /// <summary>
    /// Tạo FieldDefinition từ entity này để dùng với validation service
    /// </summary>
    public FieldDefinition ToFieldDefinition()
    {
        return new FieldDefinition(Name, FieldType, IsRequired, ValidationRules);
    }

    private CheckpointTemplateField() { } // EF Core constructor
}

/// <summary>
/// Kiểu dữ liệu của field
/// </summary>
public enum FieldValueType
{
    Text = 0,                   // Text ngắn (varchar)
    LongText = 1,               // Text dài (nvarchar(max))
    Number = 2,                 // Số thập phân (decimal)
    Integer = 3,                // Số nguyên (int)
    Boolean = 4,                // True/False
    Date = 5,                   // Ngày (date)
    DateTime = 6,               // Ngày giờ (datetime)
    MasterDataReference = 7     // FK đến bảng MasterData khác
}
