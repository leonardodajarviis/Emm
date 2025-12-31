namespace Emm.Application.Features.AppAssetAddition.Commands;

public record CreateAssetAdditionCommand(
    Guid OrganizationUnitId,
    Guid LocationId,
    string? DecisionNumber,
    DateTime? DecisionDate,
    string? Reason,
    List<CreateAssetAdditionLineCommand> AssetAdditionLines
) : IRequest<Result<object>>;

public record CreateAssetAdditionLineCommand(
    Guid AssetModelId,
    bool IsCodeGenerated,
    string? AssetCode,
    string AssetDisplayName,
    decimal UnitPrice
);
