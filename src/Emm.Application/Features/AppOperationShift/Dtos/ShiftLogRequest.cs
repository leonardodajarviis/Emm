using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Dtos;

public class CompleteShiftRequest
{
    public DateTime ActualEndTime { get; set; }
    public string? Notes { get; set; }
}

public class CancelShiftRequest
{
    public string Reason { get; set; } = null!;
}

public class AddAssetsRequest
{
    public IReadOnlyCollection<Guid> AssetIds { get; set; } = [];
}

public class AssetToAddRequest
{
    public Guid AssetId { get; set; }
    public string AssetCode { get; set; } = null!;
    public string AssetName { get; set; } = null!;
    public string AssetType { get; set; } = null!;
    public bool IsPrimary { get; set; } = false;
    public Guid? AssetGroupId { get; set; }
}

public class CreateAssetGroupRequest
{
    public required string GroupName { get; set; }
    public string? Description { get; set; }
    public BoxRole Role { get; set; } = BoxRole.Secondary;
    public int DisplayOrder { get; set; } = 0;
}

public class UpdateAssetGroupRequest
{
    public required string GroupName { get; set; }
    public string? Description { get; set; }
    public required BoxRole Role { get; set; }
    public required int DisplayOrder { get; set; }
}

public class AssignAssetToGroupRequest
{
    public required Guid AssetId { get; set; }
    public Guid? AssetGroupId { get; set; }
}
