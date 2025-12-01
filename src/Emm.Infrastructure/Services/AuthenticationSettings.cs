using Emm.Application.Abstractions;
using Emm.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Emm.Infrastructure.Services;

public class AuthenticationSettings : IAuthenticationSettings
{
    private readonly JwtOptions _jwtOptions;

    public AuthenticationSettings(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public bool IsMultiDeviceLoginAllowed => _jwtOptions.IsMultiDeviceLoginAllowed;
}
