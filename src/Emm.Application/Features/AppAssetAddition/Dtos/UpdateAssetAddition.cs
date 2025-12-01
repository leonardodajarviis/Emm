namespace Emm.Application.Features.AppAssetAddition.Dtos;

public class UpdateAssetAddition
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required long OrganizationUnitId { get; set; }
    public required long LocationId { get; set; }
    public string? DecisionNumber { get; set; }
    public string? DecisionDate { get; set; }
    public string? Reason { get; set; }
    public List<UpdateAssetAdditionLine> AssetAdditionLines { get; set; } = [];
}

public class UpdateAssetAdditionLine
{
    public long? Id { get; set; } // null for new lines
    public required long AssetModelId { get; set; }
    public required string AssetCode { get; set; }
    public required decimal UnitPrice { get; set; }
}
