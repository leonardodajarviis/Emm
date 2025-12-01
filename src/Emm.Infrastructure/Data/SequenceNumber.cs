namespace Emm.Infrastructure.Data;

public class SequenceNumber
{
    public int Id { get; set; }
    public string Prefix { get; set; } = string.Empty;
    public int NumberLength { get; set; }
    public string TableName { get; set; } = string.Empty;
    public long CurrentNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
