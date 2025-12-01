namespace Emm.Application.Abstractions;

public enum FileType
{
    Any = 0,
    Image = 1,
    Document = 2,
    Archive = 3,
    Video = 4,
    Audio = 5,
    Text = 6
}

public class FileUploadResult
{
    public Guid? FileId { get; set; }
    public bool Success { get; set; }
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public string? OriginalFileName { get; set; }
    public string? ErrorMessage { get; set; }
    public long FileSize { get; set; }
}

public class FileInfo
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ContentType { get; set; } = string.Empty;
}

public interface IFileStorage
{
    Task<FileUploadResult> SaveFileAsync(Stream fileStream, string fileName, string? subfolder = null, CancellationToken cancellationToken = default);
    Task<FileUploadResult> SaveFileAsync(byte[] fileBytes, string fileName, string? subfolder = null, CancellationToken cancellationToken = default);
    Task<FileUploadResult> SaveFileAsync(Stream fileStream, string fileName, FileType expectedFileType, string? subfolder = null, CancellationToken cancellationToken = default);
    Task<FileUploadResult> SaveFileAsync(byte[] fileBytes, string fileName, FileType expectedFileType, string? subfolder = null, CancellationToken cancellationToken = default);
    Task FileUsedAsync(Guid fileId, CancellationToken cancellationToken = default);
    string GetFileUrl(string filePath);
}
