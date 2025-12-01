using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Liên kết giữa CheckpointTemplate và AssetCategory
/// Entity thuộc Aggregate CheckpointTemplate
/// </summary>
public class CheckpointTemplateAssetCategory
{
    public long Id { get; private set; }
    public long CheckpointTemplateId { get; private set; }
    public long AssetCategoryId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? RemovedAt { get; private set; }

    public CheckpointTemplateAssetCategory(
        long checkpointTemplateId,
        long assetCategoryId)
    {
        if (checkpointTemplateId <= 0)
            throw new DomainException("Invalid checkpoint template ID");

        if (assetCategoryId <= 0)
            throw new DomainException("Invalid asset category ID");

        CheckpointTemplateId = checkpointTemplateId;
        AssetCategoryId = assetCategoryId;
        IsActive = true;
        AssignedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        RemovedAt = DateTime.UtcNow;
    }

    public void Reactivate()
    {
        IsActive = true;
        RemovedAt = null;
    }

    private CheckpointTemplateAssetCategory() { } // EF Core constructor
}
