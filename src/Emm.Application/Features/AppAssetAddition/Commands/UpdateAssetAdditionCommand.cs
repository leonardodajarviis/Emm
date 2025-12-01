namespace Emm.Application.Features.AppAssetAddition.Commands;

public record UpdateAssetAdditionCommand(
    long Id,
    string Code,
    long OrganizationUnitId,
    long LocationId,
    string? DecisionNumber,
    string? DecisionDate,
    string? Reason,
    List<UpdateAssetAdditionLineCommand> AssetAdditionLines
) : IRequest<Result<object>>;

public record UpdateAssetAdditionLineCommand(
    long? Id, // null for new lines
    long AssetModelId,
    string AssetCode,
    decimal UnitPrice
);
