namespace Emm.Application.Features.AppAssetModel.Commands;

public record RemoveMaintenancePlanCommand(
    Guid AssetModelId,
    Guid MaintenancePlanId
) : IRequest<Result<object>>;
