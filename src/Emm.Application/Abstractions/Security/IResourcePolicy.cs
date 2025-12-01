namespace Emm.Application.Abstractions.Security;

public interface IResourcePolicy<in TResource>
{
    Task<bool> CheckAsync(TResource resource, ResourceAction action, CancellationToken cancellationToken);
}
