using Emm.Domain.Entities;

namespace Emm.Application.Services;

/// <summary>
/// Service để validate các giá trị reading đảm bảo tính tịnh tiến (không giảm)
/// </summary>
public interface IReadingValueValidator
{
    /// <summary>
    /// Validate rằng giá trị reading mới tuân theo quy tắc tịnh tiến (không giảm so với giá trị trước)
    /// </summary>
    /// <param name="newValue">Giá trị reading mới cần validate</param>
    /// <param name="previousValue">Giá trị reading trước đó (nếu có)</param>
    /// <param name="assetCode">Mã asset để hiển thị trong error message</param>
    /// <param name="parameterCode">Mã parameter để hiển thị trong error message</param>
    /// <returns>Result với lỗi validation nếu giá trị mới nhỏ hơn giá trị trước</returns>
    Result ValidateMonotonicProgression(
        decimal newValue,
        decimal? previousValue,
        string assetCode,
        string parameterCode);

    /// <summary>
    /// Validate một batch readings đảm bảo tính tịnh tiến trong cả batch và so với các readings trước đó
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của reading object</typeparam>
    /// <param name="readings">Danh sách các readings cần validate</param>
    /// <param name="previousValues">Dictionary chứa giá trị cuối cùng của từng parameter từ database</param>
    /// <param name="getAssetId">Function để lấy AssetId từ reading</param>
    /// <param name="getParameterId">Function để lấy ParameterId từ reading</param>
    /// <param name="getValue">Function để lấy Value từ reading</param>
    /// <param name="getAssetCode">Function để lấy AssetCode từ reading</param>
    /// <param name="getParameterCode">Function để lấy ParameterCode từ reading</param>
    /// <returns>Result với lỗi validation nếu có bất kỳ giá trị nào vi phạm tính tịnh tiến</returns>
    Result ValidateBatchMonotonicProgression<T>(
        IEnumerable<T> readings,
        Dictionary<(Guid AssetId, Guid ParameterId), decimal> previousValues,
        Func<T, Guid> getAssetId,
        Func<T, Guid> getParameterId,
        Func<T, decimal> getValue,
        Func<T, string> getAssetCode,
        Func<T, string> getParameterCode,
        Func<T, ParameterType> getParameterType);
}

public class ReadingValueValidator : IReadingValueValidator
{
    public Result ValidateMonotonicProgression(
        decimal newValue,
        decimal? previousValue,
        string assetCode,
        string parameterCode)
    {
        // Nếu có giá trị trước đó và giá trị mới nhỏ hơn => validation error
        if (previousValue.HasValue && newValue < previousValue.Value)
        {
            return Result.Validation(
                $"Reading value for Asset {assetCode} - Parameter {parameterCode} must be greater than or equal to previous value. " +
                $"Previous: {previousValue.Value}, New: {newValue}");
        }

        return Result.Success();
    }

    public Result ValidateBatchMonotonicProgression<T>(
        IEnumerable<T> readings,
        Dictionary<(Guid AssetId, Guid ParameterId), decimal> previousValues,
        Func<T, Guid> getAssetId,
        Func<T, Guid> getParameterId,
        Func<T, decimal> getValue,
        Func<T, string> getAssetCode,
        Func<T, string> getParameterCode,
        Func<T, ParameterType> getParameterType)
    {
        // Dictionary để theo dõi giá trị cuối cùng của từng parameter trong batch hiện tại
        // Cần thiết để validate các readings của cùng parameter trong cùng batch
        var currentBatchLastValues = new Dictionary<(Guid AssetId, Guid ParameterId), decimal>();

        foreach (var reading in readings)
        {
            // Lấy các thông tin cần thiết từ reading thông qua selector functions
            if (getParameterType(reading) != ParameterType.Snapshot)
            {
                continue;
            }

            var assetId = getAssetId(reading);
            var parameterId = getParameterId(reading);
            var value = getValue(reading);
            var assetCode = getAssetCode(reading);
            var parameterCode = getParameterCode(reading);

            // Tạo key để tra cứu trong dictionary
            var parameterKey = (assetId, parameterId);
            decimal? previousValue = null;

            // Bước 1: Kiểm tra xem parameter này đã có reading nào trong batch hiện tại chưa
            // Nếu có thì lấy giá trị cuối cùng trong batch để so sánh
            if (currentBatchLastValues.TryGetValue(parameterKey, out var batchValue))
            {
                previousValue = batchValue;
            }
            // Bước 2: Nếu không có trong batch, kiểm tra giá trị cuối cùng từ database
            // Đây là giá trị của lần reading trước đó (ngoài batch hiện tại)
            else if (previousValues.TryGetValue(parameterKey, out var dbValue))
            {
                previousValue = dbValue;
            }

            // Validate: Giá trị mới phải >= giá trị trước đó (tính tịnh tiến)
            var validationResult = ValidateMonotonicProgression(value, previousValue, assetCode, parameterCode);
            if (!validationResult.IsSuccess)
            {
                // Nếu validation fail, return ngay lập tức với error message
                return validationResult;
            }

            // Cập nhật giá trị cuối cùng của parameter này trong batch
            // Để các reading tiếp theo của cùng parameter có thể so sánh
            currentBatchLastValues[parameterKey] = value;
        }

        // Tất cả readings đều pass validation
        return Result.Success();
    }
}
