using Emm.Domain.Abstractions;

namespace Emm.Domain.Warehouse;

public class Uow : AggregateRoot
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}