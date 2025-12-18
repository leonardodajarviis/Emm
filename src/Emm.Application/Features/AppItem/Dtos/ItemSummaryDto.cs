namespace Emm.Application.Features.AppItem.Dtos;

public class ItemSummaryDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Guid UnitOfMeasureId { get; set; }
    public string UnitOfMeasureName { get; set; } = null!;
}
