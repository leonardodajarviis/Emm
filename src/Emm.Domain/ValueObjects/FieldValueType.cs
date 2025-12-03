namespace Emm.Domain.ValueObjects;

/// <summary>
/// Enum định nghĩa các kiểu giá trị cho field trong EAV pattern
/// </summary>
public enum FieldValueType
{
    Text = 1,
    LongText = 2,
    Number = 3,
    Integer = 4,
    Boolean = 5,
    Date = 6,
    DateTime = 7,
    MasterDataReference = 8
}
