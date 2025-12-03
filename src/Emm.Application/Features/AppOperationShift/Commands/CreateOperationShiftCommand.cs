using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record CreateOperationShiftCommand(
    string Name,
    string? Notes,
    IReadOnlyCollection<AssetAssignmentRequest>? Assets,
    IReadOnlyCollection<AssetGroupRequest>? AssetGroups
) : IRequest<Result<object>>;

/// <summary>
/// Request để gán asset vào ca vận hành
/// Có thể gán trực tiếp hoặc gán vào group thông qua GroupLinkedId
/// </summary>
public sealed record AssetAssignmentRequest
{
    /// <summary>
    /// ID của asset cần thêm vào ca
    /// </summary>
    public long AssetId { get; init; }

    /// <summary>
    /// Đánh dấu asset này là primary asset
    /// </summary>
    public bool IsPrimary { get; init; }

    /// <summary>
    /// UUID tạm thời của group (nếu muốn gán vào group)
    /// LinkedId này phải match với LinkedId trong AssetGroups
    /// </summary>
    public Guid? GroupLinkedId { get; init; }
}

/// <summary>
/// Request để tạo asset group trong ca vận hành
/// </summary>
public sealed record AssetGroupRequest
{
    /// <summary>
    /// UUID tạm thời do client tạo để link với assets
    /// </summary>
    public Guid LinkedId { get; init; }

    /// <summary>
    /// Tên nhóm thiết bị
    /// </summary>
    public string GroupName { get; init; } = null!;

    /// <summary>
    /// Vai trò của nhóm
    /// </summary>
    public GroupRole Role { get; init; }

    /// <summary>
    /// Thứ tự hiển thị
    /// </summary>
    public int DisplayOrder { get; init; }

    /// <summary>
    /// Mô tả nhóm (optional)
    /// </summary>
    public string? Description { get; init; }
}
