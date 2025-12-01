namespace Emm.Application.Features.AppAssetAddition.Commands;

public record CreateAssetAdditionCommand(
    long OrganizationUnitId,
    long LocationId,
    string? DecisionNumber,
    string? DecisionDate,
    string? Reason,
    List<CreateAssetAdditionLineCommand> AssetAdditionLines
) : IRequest<Result<object>>;

public record CreateAssetAdditionLineCommand(
    long AssetModelId,
    string AssetCode,
    decimal UnitPrice
);
