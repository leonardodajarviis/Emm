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
    public IReadOnlyCollection<long> AssetIds { get; set; } = [];
}

public class AssetToAddRequest
{
    public long AssetId { get; set; }
    public string AssetCode { get; set; } = null!;
    public string AssetName { get; set; } = null!;
    public string AssetType { get; set; } = null!;
    public bool IsPrimary { get; set; } = false;
}
