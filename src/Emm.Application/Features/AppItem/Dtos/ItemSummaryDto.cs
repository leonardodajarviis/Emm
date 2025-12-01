namespace Emm.Application.Features.AppItem.Dtos;

public class ItemSummaryDto
{
    public long Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public long UnitOfMeasureId { get; set; }
    public string UnitOfMeasureName { get; set; } = null!;
}
