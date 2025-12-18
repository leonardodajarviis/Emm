namespace Emm.Application.Features.AppAssetAddition.Commands;

public record UpdateAssetAdditionCommand(
    Guid Id,
    string Code,
    Guid OrganizationUnitId,
    Guid LocationId,
    string? DecisionNumber,
    string? DecisionDate,
    string? Reason,
    List<UpdateAssetAdditionLineCommand> AssetAdditionLines
) : IRequest<Result<object>>;

public record UpdateAssetAdditionLineCommand(
    long? Id, // null for new lines
    Guid AssetModelId,
    string AssetCode,
    decimal UnitPrice
);
