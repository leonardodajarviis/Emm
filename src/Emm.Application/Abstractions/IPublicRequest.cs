namespace Emm.Application.Abstractions;

/// <summary>
/// Marker interface for requests that don't require authentication.
/// Commands/queries implementing this interface will skip authorization checks.
/// Used for public endpoints like Login, Register, Password Reset, etc.
/// </summary>
public interface IPublicRequest
{
}
