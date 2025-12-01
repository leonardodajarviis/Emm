namespace Emm.Infrastructure.Options;

public class LocalFileStorageOptions
{
    public const string SectionName = "LocalFileStorage";
    
    public string RootPath { get; set; } = "uploads";
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10 MB default
    public string[] AllowedExtensions { get; set; } = [];
    public string BaseUrl { get; set; } = "/files";
    public bool ValidateFileType { get; set; } = true; // Enable file type validation by default
}
