namespace Emm.Application.Features.AppAssetModel.Commands;

public record RemoveMaintenancePlanCommand(
    long AssetModelId,
    long MaintenancePlanId
) : IRequest<Result<object>>;
