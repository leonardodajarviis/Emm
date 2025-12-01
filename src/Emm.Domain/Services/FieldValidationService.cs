using Emm.Domain.Entities.Operations;
using Emm.Domain.ValueObjects;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Emm.Domain.Services;

/// <summary>
/// Domain Service - Validate field values cho EAV pattern
/// </summary>
public class FieldValidationService : IFieldValidationService
{
    public ValidationResult ValidateValue(
        string? value,
        string fieldName,
        FieldValueType fieldType,
        bool isRequired,
        string? validationRulesJson = null)
    {
        // Check required
        if (isRequired && string.IsNullOrWhiteSpace(value))
            return ValidationResult.Failure($"Field '{fieldName}' is required");

        // Nếu không required và không có value thì OK
        if (string.IsNullOrWhiteSpace(value))
            return ValidationResult.Success();

        // Validate theo FieldType
        var typeValidation = ValidateByType(value, fieldName, fieldType);
        if (!typeValidation.IsValid)
            return typeValidation;

        // Validate theo ValidationRules nếu có
        if (!string.IsNullOrWhiteSpace(validationRulesJson))
        {
            var rulesValidation = ValidateByRules(value, fieldName, fieldType, validationRulesJson);
            if (!rulesValidation.IsValid)
                return rulesValidation;
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateValue(string? value, FieldDefinition fieldDefinition)
    {
        return ValidateValue(
            value,
            fieldDefinition.Name,
            fieldDefinition.FieldType,
            fieldDefinition.IsRequired,
            fieldDefinition.ValidationRulesJson);
    }

    public ValidationResult ValidateRulesJson(string? rulesJson)
    {
        if (string.IsNullOrWhiteSpace(rulesJson))
            return ValidationResult.Success();

        try
        {
            JsonDocument.Parse(rulesJson);
            return ValidationResult.Success();
        }
        catch (JsonException ex)
        {
            return ValidationResult.Failure($"ValidationRules must be valid JSON: {ex.Message}");
        }
    }

    private ValidationResult ValidateByType(string value, string fieldName, FieldValueType fieldType)
    {
        try
        {
            switch (fieldType)
            {
                case FieldValueType.Text:
                case FieldValueType.LongText:
                    return ValidationResult.Success();

                case FieldValueType.Number:
                    if (!decimal.TryParse(value, out _))
                        return ValidationResult.Failure($"Field '{fieldName}' must be a valid number");
                    break;

                case FieldValueType.Integer:
                    if (!int.TryParse(value, out _))
                        return ValidationResult.Failure($"Field '{fieldName}' must be a valid integer");
                    break;

                case FieldValueType.Boolean:
                    if (!bool.TryParse(value, out _))
                        return ValidationResult.Failure($"Field '{fieldName}' must be true or false");
                    break;

                case FieldValueType.Date:
                    if (!DateTime.TryParse(value, out _))
                        return ValidationResult.Failure($"Field '{fieldName}' must be a valid date");
                    break;

                case FieldValueType.DateTime:
                    if (!DateTime.TryParse(value, out _))
                        return ValidationResult.Failure($"Field '{fieldName}' must be a valid datetime");
                    break;

                case FieldValueType.MasterDataReference:
                    if (!long.TryParse(value, out _))
                        return ValidationResult.Failure($"Field '{fieldName}' must be a valid ID");
                    break;
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            return ValidationResult.Failure($"Validation error for field '{fieldName}': {ex.Message}");
        }
    }

    private ValidationResult ValidateByRules(string value, string fieldName, FieldValueType fieldType, string rulesJson)
    {
        try
        {
            var rules = JsonDocument.Parse(rulesJson);

            // Min/Max cho số
            if (fieldType == FieldValueType.Number || fieldType == FieldValueType.Integer)
            {
                var numValue = decimal.Parse(value);

                if (rules.RootElement.TryGetProperty("min", out var minElement))
                {
                    var min = minElement.GetDecimal();
                    if (numValue < min)
                        return ValidationResult.Failure($"Field '{fieldName}' must be >= {min}");
                }

                if (rules.RootElement.TryGetProperty("max", out var maxElement))
                {
                    var max = maxElement.GetDecimal();
                    if (numValue > max)
                        return ValidationResult.Failure($"Field '{fieldName}' must be <= {max}");
                }
            }

            // MinLength/MaxLength cho text
            if (fieldType == FieldValueType.Text || fieldType == FieldValueType.LongText)
            {
                if (rules.RootElement.TryGetProperty("minLength", out var minLengthElement))
                {
                    var minLength = minLengthElement.GetInt32();
                    if (value.Length < minLength)
                        return ValidationResult.Failure($"Field '{fieldName}' must be at least {minLength} characters");
                }

                if (rules.RootElement.TryGetProperty("maxLength", out var maxLengthElement))
                {
                    var maxLength = maxLengthElement.GetInt32();
                    if (value.Length > maxLength)
                        return ValidationResult.Failure($"Field '{fieldName}' must be at most {maxLength} characters");
                }

                // Regex pattern
                if (rules.RootElement.TryGetProperty("regex", out var regexElement))
                {
                    var pattern = regexElement.GetString();
                    if (!string.IsNullOrEmpty(pattern) && !Regex.IsMatch(value, pattern))
                        return ValidationResult.Failure($"Field '{fieldName}' does not match required pattern");
                }
            }

            // Date range validation
            if (fieldType == FieldValueType.Date || fieldType == FieldValueType.DateTime)
            {
                var dateValue = DateTime.Parse(value);

                if (rules.RootElement.TryGetProperty("minDate", out var minDateElement))
                {
                    if (DateTime.TryParse(minDateElement.GetString(), out var minDate))
                    {
                        if (dateValue < minDate)
                            return ValidationResult.Failure($"Field '{fieldName}' must be after {minDate:yyyy-MM-dd}");
                    }
                }

                if (rules.RootElement.TryGetProperty("maxDate", out var maxDateElement))
                {
                    if (DateTime.TryParse(maxDateElement.GetString(), out var maxDate))
                    {
                        if (dateValue > maxDate)
                            return ValidationResult.Failure($"Field '{fieldName}' must be before {maxDate:yyyy-MM-dd}");
                    }
                }
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            return ValidationResult.Failure($"Validation rule error for field '{fieldName}': {ex.Message}");
        }
    }
}
