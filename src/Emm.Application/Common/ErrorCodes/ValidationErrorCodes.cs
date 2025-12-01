namespace Emm.Application.Common.ErrorCodes;

/// <summary>
/// Validation error codes
/// </summary>
public static class ValidationErrorCodes
{
    public const string FieldRequired = "FIELD_REQUIRED";
    public const string FieldInvalidFormat = "FIELD_INVALID_FORMAT";
    public const string FieldTooLong = "FIELD_TOO_LONG";
    public const string FieldTooShort = "FIELD_TOO_SHORT";
    public const string FieldOutOfRange = "FIELD_OUT_OF_RANGE";
    public const string FieldDuplicate = "FIELD_DUPLICATE";
    public const string FieldInvalidValue = "FIELD_INVALID_VALUE";
    public const string DateInvalid = "DATE_INVALID";
    public const string DateRangeInvalid = "DATE_RANGE_INVALID";
    public const string EmailInvalid = "EMAIL_INVALID";
    public const string PhoneInvalid = "PHONE_INVALID";
    public const string UrlInvalid = "URL_INVALID";
    public const string NumberInvalid = "NUMBER_INVALID";
}
