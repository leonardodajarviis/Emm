namespace Emm.Application.Features.AppAssetAddition.Dtos;

public class AssetAdditionResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required long OrganizationUnitId { get; set; }
    public required long LocationId { get; set; }
    public string? DecisionNumber { get; set; }
    public DateTime? DecisionDate { get; set; }
    public string? Reason { get; set; }
    public required DateTime CreatedAt { get; set; }
    public List<AssetAdditionLineResponse> AssetAdditionLines { get; set; } = [];
}

public class AssetAdditionLineResponse
{
    public required long Id { get; set; }
    public required long AssetAdditionId { get; set; }
    public required long AssetModelId { get; set; }
    public required string AssetCode { get; set; }
    public required decimal UnitPrice { get; set; }
}
