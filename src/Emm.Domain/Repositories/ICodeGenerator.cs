using Emm.Domain.ValueObjects;

namespace Emm.Domain.Repositories;

public interface ICodeGenerator
{
    Task<string> GenerateNextCodeAsync(string prefix, string tableName, int maxLength = 6, CancellationToken cancellationToken = default);
    Task<NaturalKey> GetNaturalKeyAsync(
        string prefix,
        string tableName,
        int numberLength = 6,
        CancellationToken cancellationToken = default);
}
