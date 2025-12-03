using Emm.Application.Common;
using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record CreateAssetGroupCommand(
    long OperationShiftId,
    string GroupName,
    GroupRole Role,
    int DisplayOrder,
    string? Description,
    IReadOnlyCollection<long>? AssetIds) : IRequest<Result<CreateAssetGroupResult>>;

/// <summary>
/// Kết quả sau khi tạo asset group
/// </summary>
public sealed record CreateAssetGroupResult
{
    /// <summary>
    /// ID của group vừa tạo
    /// </summary>
    public long GroupId { get; init; }

    /// <summary>
    /// LinkedId của group (UUID)
    /// </summary>
    public Guid LinkedId { get; init; }

    /// <summary>
    /// Danh sách asset IDs đã được thêm vào group
    /// </summary>
    public IReadOnlyCollection<long> AssignedAssetIds { get; init; } = Array.Empty<long>();

    /// <summary>
    /// Danh sách asset IDs đã tồn tại trong shift trước đó
    /// </summary>
    public IReadOnlyCollection<long> ExistingAssetIds { get; init; } = Array.Empty<long>();

    /// <summary>
    /// Danh sách asset IDs mới được thêm vào shift
    /// </summary>
    public IReadOnlyCollection<long> NewAssetIds { get; init; } = Array.Empty<long>();
}
