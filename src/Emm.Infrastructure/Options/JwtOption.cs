namespace Emm.Infrastructure.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string AccessKey { get; set; } = string.Empty;
    public string RefreshKey { get; set; } = string.Empty;
    public int AccessExpiresIn { get; set; } = 0;
    public int RefreshExpiresIn { get; set; } = 0;
    public bool IsMultiDeviceLoginAllowed { get; set; } = true;
}
