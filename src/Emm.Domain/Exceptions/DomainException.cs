namespace Emm.Domain.Exceptions;

public class DomainException : Exception
{
    public string? Rule { get; }
    public DomainException(string message, string? rule = null) : base(message)
    {
        Rule = rule;
    }

    public DomainException(string message, Exception innerException, string? rule = null)
        : base(message, innerException)
    {
        Rule = rule;
    }
}
