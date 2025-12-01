using Emm.Domain.Entities.Operations;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Services;

/// <summary>
/// Domain Service - Validate field values cho EAV pattern
/// Có thể tái sử dụng cho nhiều loại entity khác nhau
/// </summary>
public interface IFieldValidationService
{
    /// <summary>
    /// Validate giá trị theo field type và validation rules
    /// </summary>
    /// <param name="value">Giá trị cần validate</param>
    /// <param name="fieldName">Tên field (dùng cho error message)</param>
    /// <param name="fieldType">Kiểu dữ liệu</param>
    /// <param name="isRequired">Field có bắt buộc không</param>
    /// <param name="validationRulesJson">JSON chứa các validation rules</param>
    /// <returns>Kết quả validation</returns>
    ValidationResult ValidateValue(
        string? value,
        string fieldName,
        FieldValueType fieldType,
        bool isRequired,
        string? validationRulesJson = null);

    /// <summary>
    /// Validate giá trị theo một field definition cụ thể
    /// </summary>
    ValidationResult ValidateValue(string? value, FieldDefinition fieldDefinition);

    /// <summary>
    /// Validate JSON format của validation rules
    /// </summary>
    ValidationResult ValidateRulesJson(string? rulesJson);
}
