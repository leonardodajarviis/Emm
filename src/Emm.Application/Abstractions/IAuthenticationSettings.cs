namespace Emm.Application.Abstractions;

/// <summary>
/// Authentication settings interface to avoid direct dependency on Infrastructure
/// </summary>
public interface IAuthenticationSettings
{
    /// <summary>
    /// Whether to allow multiple device logins for a single user
    /// </summary>
    bool IsMultiDeviceLoginAllowed { get; }
}
