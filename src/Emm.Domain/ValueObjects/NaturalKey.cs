using System.Text.RegularExpressions;
using Emm.Domain.Exceptions;

namespace Emm.Domain.ValueObjects;

public readonly record struct NaturalKey : IEquatable<NaturalKey>
{
    public string Value { get; }

    // Chuyển sang Nullable vì mã tự nhập sẽ không có các thông tin này
    public string? Prefix { get; }
    public int? Number { get; }
    public int? Padding { get; }

    // Flag để kiểm tra xem đây là mã hệ thống hay mã người dùng
    public bool IsSystemGenerated => Number.HasValue;

    // Constructor Private
    private NaturalKey(string value, string? prefix, int? number, int? padding)
    {
        Value = value;
        Prefix = prefix;
        Number = number;
        Padding = padding;
    }

    // --- CASE 1: Mã hệ thống tự sinh (Strict Format) ---
    public static NaturalKey Create(string prefix, int number, int padding)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new DomainException("Prefix cannot be empty");

        prefix = prefix.Trim().ToUpperInvariant();

        if (!Regex.IsMatch(prefix, "^[A-Z]+$"))
            throw new DomainException("Prefix must contain only uppercase letters A-Z");

        if (number < 0)
            throw new DomainException("Number cannot be negative");

        var value = $"{prefix}-{number.ToString().PadLeft(padding, '0')}";

        return new NaturalKey(value, prefix, number, padding);
    }

    // --- CASE 2: Mã người dùng tự nhập (Free Text) ---
    public static NaturalKey CreateRaw(string rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            throw new DomainException("Value cannot be empty");

        rawValue = rawValue.Trim().ToUpperInvariant(); // Vẫn nên chuẩn hóa viết hoa

        // Validate cơ bản (độ dài, ký tự đặc biệt nếu cần)
        if (rawValue.Length > 50)
            throw new DomainException("Key is too long");

        // Kiểm tra xem user có cố tình nhập đúng format hệ thống không?
        // Nếu muốn user nhập "ORD-001" thì vẫn tính là system key -> gọi logic Parse
        // Ở đây tôi giả sử nếu dùng CreateRaw thì coi như là Raw (Prefix = null)
        return new NaturalKey(rawValue, null, null, null);
    }

    // --- PARSE LOGIC (Thông minh hơn) ---
    public static NaturalKey Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Value cannot be empty");

        // Logic cũ: Cố gắng parse theo format Prefix-Number
        // Regex check: Bắt đầu bằng Chữ, gạch ngang, kết thúc bằng Số
        var match = Regex.Match(value, @"^([A-Z]+)-(\d+)$");

        if (match.Success)
        {
            var prefix = match.Groups[1].Value;
            var numberPart = match.Groups[2].Value;

            if (int.TryParse(numberPart, out int number))
            {
                var padding = numberPart.Length;
                // Trả về dạng System Key (có đầy đủ thông tin để sort/increment sau này)
                return new NaturalKey(value, prefix, number, padding);
            }
        }

        // Nếu không đúng format hệ thống -> Coi như là Mã người dùng tự nhập (Raw)
        // Fallback về CreateRaw
        return CreateRaw(value);
    }

    public override string ToString() => Value;
    public override int GetHashCode() => Value.GetHashCode();
}
