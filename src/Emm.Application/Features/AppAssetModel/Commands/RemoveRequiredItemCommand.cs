namespace Emm.Application.Features.AppAssetModel.Commands;

public record RemoveRequiredItemFromMaintenancePlanCommand(
    long AssetModelId,
    long MaintenancePlanId,
    long RequiredItemId
) : IRequest<Result<object>>;
