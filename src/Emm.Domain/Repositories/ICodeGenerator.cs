namespace Emm.Domain.Repositories;

public interface ICodeGenerator
{
    Task<string> GenerateNextCodeAsync(string prefix, string tableName, int maxLength = 6, CancellationToken cancellationToken = default);
}
