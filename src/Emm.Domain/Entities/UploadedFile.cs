using Emm.Domain.Abstractions;

namespace Emm.Domain.Entities;

public class UploadedFile : AggregateRoot
{
    public string OriginalFileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string? Subfolder { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public bool IsUsed { get; private set; }

    private UploadedFile() { } // For EF Core

    public UploadedFile(
        string originalFileName,
        string filePath,
        string contentType,
        long fileSize,
        string? subfolder = null)
    {
        OriginalFileName = originalFileName;
        FilePath = filePath;
        ContentType = contentType;
        FileSize = fileSize;
        Subfolder = subfolder;
        UploadedAt = DateTime.UtcNow;
        IsUsed = false;
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
    }
}
