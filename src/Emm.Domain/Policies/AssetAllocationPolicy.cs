using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.AssetCatalog.BusinessRules;
using Emm.Domain.Exceptions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Policies;

public class AssetAllocationPolicy
{
    public void EnsureCanAllocate(IEnumerable<Asset> assets)
    {
        if (!assets.Any())
            throw new DomainException("Không có tài");

        var invalid = assets.Where(a => a.Status != AssetStatus.Idle).ToList();

        if (invalid.Any())
        {
            throw new DomainException(
                $"Tài sản không rảnh: {string.Join(", ", invalid.Select(x => x.Code))}",
                AssetRules.AssetNotIdle
            );
        }
    }
}
