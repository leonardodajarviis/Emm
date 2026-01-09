using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Commands;

// public record CreateOperationShiftCommand(
//     string Name,
//     string? Notes,
//     IReadOnlyCollection<AssetAssignmentRequest>? Assets,
//     IReadOnlyCollection<AssetBoxRequest>? AssetBoxes
// ) : IRequest<Result<object>>;

public record CreateOperationShiftCommand : IRequest<Result>
{
    public string Name {get; init; } = null!;
    public string? Notes { get; init; }
    public string? Description { get; init; }
    public IReadOnlyCollection<AssetAssignmentRequest> Assets { get; init; } = [];
    public IReadOnlyCollection<AssetBoxRequest> AssetBoxes { get; init; } = [];
}


/// <summary>
/// Request để gán asset vào ca vận hành
/// Có thể gán trực tiếp hoặc gán vào group thông qua GroupLinkedId
/// </summary>
public sealed record AssetAssignmentRequest
{
    /// <summary>
    /// ID của asset cần thêm vào ca
    /// </summary>
    public Guid AssetId { get; init; }

    /// <summary>
    /// Đánh dấu asset này là primary asset
    /// </summary>
    public bool IsPrimary { get; init; }
}

/// <summary>
/// Request để tạo asset group trong ca vận hành
/// </summary>
public sealed record AssetBoxRequest
{
    /// <summary>
    /// Tên nhóm thiết bị
    /// </summary>
    public string BoxName { get; init; } = null!;

    public IEnumerable<Guid>? AssetIds { get; init; }

    /// <summary>
    /// Vai trò của nhóm
    /// </summary>
    public BoxRole Role { get; init; }

    /// <summary>
    /// Thứ tự hiển thị
    /// </summary>
    public int DisplayOrder { get; init; }

    /// <summary>
    /// Mô tả nhóm (optional)
    /// </summary>
    public string? Description { get; init; }
}
