using Emm.Domain.ValueObjects;

namespace Emm.Domain.Repositories;

public interface ICodeGenerator
{
    Task<string> GenerateNextCodeAsync<TEntity>(string prefix, int maxLength = 6, CancellationToken cancellationToken = default)
        where TEntity : class;

    Task<NaturalKey> GetNaturalKeyAsync<TEntity>(
        string prefix,
        int numberLength = 6,
        CancellationToken cancellationToken = default)
        where TEntity : class;
}
