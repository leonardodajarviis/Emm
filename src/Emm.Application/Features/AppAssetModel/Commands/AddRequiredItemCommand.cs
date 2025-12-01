namespace Emm.Application.Features.AppAssetModel.Commands;

public record AddRequiredItemToMaintenancePlanCommand(
    long AssetModelId,
    long MaintenancePlanId,
    long ItemId,
    decimal Quantity,
    bool IsRequired,
    string? Note = null
) : IRequest<Result<object>>;
