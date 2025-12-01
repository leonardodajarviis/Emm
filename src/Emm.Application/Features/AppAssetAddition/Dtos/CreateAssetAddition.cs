namespace Emm.Application.Features.AppAssetAddition.Dtos;

public class CreateAssetAddition
{
    public required string Code { get; set; }
    public required long OrganizationUnitId { get; set; }
    public required long LocationId { get; set; }
    public string? DecisionNumber { get; set; }
    public string? DecisionDate { get; set; }
    public string? Reason { get; set; }
    public List<CreateAssetAdditionLine> AssetAdditionLines { get; set; } = [];
}

public class CreateAssetAdditionLine
{
    public required long AssetModelId { get; set; }
    public required string AssetCode { get; set; }
    public required decimal UnitPrice { get; set; }
}
