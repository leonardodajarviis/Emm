using System.Reflection;
using System.Text.Json;
using Emm.Application.Common.ErrorCodes;

namespace Emm.Tools.ErrorCodeGenerator;

public class Program
{
    public static async Task Main(string[] args)
    {
        var outputPath = args.Length > 0 ? args[0] : "error-codes.json";

        Console.WriteLine("🔍 Scanning error codes...");

        var assembly = typeof(GeneralErrorCodes).Assembly;
        var errorCodeTypes = assembly.GetTypes()
            .Where(t => t.IsClass &&
                        t.IsAbstract &&
                        t.IsSealed && // static class
                        t.Namespace?.Contains("ErrorCodes") == true)
            .OrderBy(t => t.Name);

        // Simple flat dictionary: code -> empty message
        var codes = new SortedDictionary<string, string>();

        foreach (var type in errorCodeTypes)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

            foreach (var field in fields)
            {
                var code = field.GetValue(null)?.ToString();
                if (!string.IsNullOrEmpty(code) && !codes.ContainsKey(code))
                {
                    codes[code] = "";
                }
            }
        }

        // Scan thêm các business rule codes từ DomainErrorCodeMapper
        ScanBusinessRuleCodes(codes);

        var json = JsonSerializer.Serialize(codes, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(outputPath, json);

        Console.WriteLine($"✅ Generated {codes.Count} error codes to: {Path.GetFullPath(outputPath)}");
        Console.WriteLine();

        // Summary by prefix
        var summary = codes.Keys
            .GroupBy(c => c.Split('_')[0])
            .OrderBy(g => g.Key);

        Console.WriteLine("📊 Summary by category:");
        foreach (var group in summary)
        {
            Console.WriteLine($"   {group.Key}: {group.Count()} codes");
        }
    }

    private static void ScanBusinessRuleCodes(SortedDictionary<string, string> codes)
    {
        // Các business rule codes được định nghĩa trong DomainErrorCodeMapper
        var businessRuleCodes = new[]
        {
            // Asset rules
            "ASSET_CANNOT_MODIFY_DISPOSED",
            "ASSET_MUST_HAVE_TYPE",
            "ASSET_INVALID_STATE",

            // Shift rules
            "SHIFT_CANNOT_START_IN_FUTURE",
            "SHIFT_END_TIME_BEFORE_START",
            "SHIFT_END_TIME_MUST_BE_AFTER_START",
            "SHIFT_DURATION_EXCEEDS_24_HOURS",
            "SHIFT_CANNOT_CHANGE_SCHEDULE_IN_PROGRESS",
            "SHIFT_ONLY_ONE_PRIMARY_ASSET",
            "SHIFT_INVALID_STATE",
            "SHIFT_INVALID_TIME",
            "SHIFT_ASSET_NOT_FOUND",
            "SHIFT_NO_ASSETS",
            "SHIFT_LOG_MISMATCH",
            "SHIFT_LOG_CANNOT_REMOVE_IN_PROGRESS",

            // User rules
            "USER_MUST_HAVE_ROLE",
            "USER_CANNOT_DELETE_SELF",
            "USER_INVALID_STATE",
        };

        foreach (var code in businessRuleCodes)
        {
            if (!codes.ContainsKey(code))
            {
                codes[code] = "";
            }
        }
    }
}
