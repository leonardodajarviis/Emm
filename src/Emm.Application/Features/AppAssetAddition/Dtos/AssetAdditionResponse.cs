namespace Emm.Application.Features.AppAssetAddition.Dtos;

public class AssetAdditionSummaryResponse
{
    public required Guid Id { get; set; }
    public required string Code { get; set; }
    public required Guid OrganizationUnitId { get; set; }
    public string OrganizationUnitName { get; set; } = null!;
    public required Guid LocationId { get; set; }
    public string LocationName { get; set; } = null!;
    public string? DecisionNumber { get; set; }
    public DateTime? DecisionDate { get; set; }
    public string? Reason { get; set; }
    public required DateTime CreatedAt { get; set; }
}

public class AssetAdditionResponse
{
    public required Guid Id { get; set; }
    public required string Code { get; set; }
    public required Guid OrganizationUnitId { get; set; }
    public string OrganizationUnitName { get; set; } = null!;
    public required Guid LocationId { get; set; }
    public string LocationName { get; set; } = null!;
    public string? DecisionNumber { get; set; }
    public DateTime? DecisionDate { get; set; }
    public string? Reason { get; set; }
    public required DateTime CreatedAt { get; set; }
    public List<AssetAdditionLineResponse> AssetAdditionLines { get; set; } = [];
}

public class AssetAdditionLineResponse
{
    public required Guid Id { get; set; }
    public required Guid AssetAdditionId { get; set; }
    public required Guid AssetModelId { get; set; }
    public required string? AssetCode { get; set; }
    public required decimal UnitPrice { get; set; }
}
