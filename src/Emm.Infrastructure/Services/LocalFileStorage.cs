using Emm.Application.Abstractions;
using Emm.Domain.Entities;
using Emm.Infrastructure.Data;
using Emm.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Emm.Infrastructure.Services;

public class LocalFileStorage : IFileStorage
{
    private readonly LocalFileStorageOptions _options;
    private readonly XDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    // Cache để tránh phải kiểm tra directory exists nhiều lần
    private readonly ConcurrentDictionary<string, bool> _directoryCache = new();

    // Static readonly dictionaries for better performance
    private static readonly Dictionary<string, string> _contentTypeMappings = new()
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".bmp", "image/bmp" },
        { ".webp", "image/webp" },
        { ".svg", "image/svg+xml" },
        { ".pdf", "application/pdf" },
        { ".doc", "application/msword" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".xls", "application/vnd.ms-excel" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".txt", "text/plain" },
        { ".csv", "text/csv" },
        { ".json", "application/json" },
        { ".xml", "application/xml" },
        { ".zip", "application/zip" },
        { ".rar", "application/x-rar-compressed" },
        { ".7z", "application/x-7z-compressed" }
    };

    private static readonly HashSet<string> _imageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg"
    };

    private static readonly HashSet<string> _documentExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".csv", ".json", ".xml"
    };

    private static readonly HashSet<string> _archiveExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".zip", ".rar", ".7z", ".tar", ".gz"
    };

    private static readonly HashSet<string> _videoExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".m4v"
    };

    private static readonly HashSet<string> _audioExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a"
    };

    private static readonly HashSet<string> _textExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".txt", ".csv", ".json", ".xml", ".html", ".css", ".js", ".md", ".log"
    };

    public LocalFileStorage(
        IOptions<LocalFileStorageOptions> options,
        XDbContext db,
        IHttpContextAccessor httpContextAccessor)
    {
        _options = options.Value;
        _db = db;
        _httpContextAccessor = httpContextAccessor;

        EnsureDirectoryExists(_options.RootPath);
    }

    public async Task<FileUploadResult> SaveFileAsync(Stream fileStream, string fileName, string? subfolder = null, CancellationToken cancellationToken = default)
    {
        var validation = ValidateFileInput(fileStream, fileName);
        if (!validation.IsValid)
        {
            return CreateErrorResult(validation.ErrorMessage);
        }

        try
        {
            return await ProcessFileUploadAsync(fileStream, fileName, null, subfolder, cancellationToken);
        }
        catch
        {
            return CreateErrorResult("An error occurred while saving the file");
        }
    }

    public async Task<FileUploadResult> SaveFileAsync(byte[] fileBytes, string fileName, string? subfolder = null, CancellationToken cancellationToken = default)
    {
        if (fileBytes == null || fileBytes.Length == 0)
        {
            return CreateErrorResult("File bytes are empty or null");
        }

        using var memoryStream = new MemoryStream(fileBytes);
        return await SaveFileAsync(memoryStream, fileName, subfolder, cancellationToken);
    }

    public async Task<FileUploadResult> SaveFileAsync(Stream fileStream, string fileName, FileType expectedFileType, string? subfolder = null, CancellationToken cancellationToken = default)
    {
        var validation = ValidateFileInput(fileStream, fileName);
        if (!validation.IsValid)
        {
            return CreateErrorResult(validation.ErrorMessage);
        }

        try
        {
            return await ProcessFileUploadAsync(fileStream, fileName, expectedFileType, subfolder, cancellationToken);
        }
        catch
        {
            return CreateErrorResult("An error occurred while saving the file");
        }
    }

    public async Task<FileUploadResult> SaveFileAsync(byte[] fileBytes, string fileName, FileType expectedFileType, string? subfolder = null, CancellationToken cancellationToken = default)
    {
        if (fileBytes == null || fileBytes.Length == 0)
        {
            return CreateErrorResult("File bytes are empty or null");
        }

        using var memoryStream = new MemoryStream(fileBytes);
        return await SaveFileAsync(memoryStream, fileName, expectedFileType, subfolder, cancellationToken);
    }

    public async Task FileUsedAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var uploadedFile = await _db.UploadedFiles.FindAsync(fileId);
        if (uploadedFile != null && !uploadedFile.IsUsed)
        {
            uploadedFile.MarkAsUsed();
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public string GetFileUrl(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return string.Empty;

        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null) return string.Empty;

        var domain = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

        return $"{domain}{_options.BaseUrl.TrimEnd('/')}/{filePath.TrimStart('/')}";
    }

    public bool ValidateFileType(Stream fileStream, string fileName)
    {
        return IsValidFileType(fileStream, fileName);
    }

    public string GetDetectedFileType(Stream fileStream)
    {
        if (fileStream == null || fileStream.Length == 0)
            return string.Empty;

        if (TryReadFromStream(fileStream, buffer => GetFileTypeFromMagicBytes(buffer), out string detectedType))
        {
            return detectedType ?? string.Empty;
        }

        return string.Empty;
    }

    // Phương thức helper để đọc từ stream một cách an toàn
    private bool TryReadFromStream<T>(Stream fileStream, Func<byte[], T> processor, out T result)
    {

        result = default!;
        var originalPosition = fileStream.Position;

        try
        {
            fileStream.Position = 0;
            var buffer = new byte[8];
            var bytesRead = fileStream.Read(buffer, 0, buffer.Length);

            if (bytesRead < 2)
                return false;

            result = processor(buffer);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            fileStream.Position = originalPosition;
        }
    }

    private async Task<FileUploadResult> ProcessFileUploadAsync(Stream fileStream, string fileName, FileType? expectedFileType, string? subfolder, CancellationToken cancellationToken)
    {
        var sanitizedFileName = SanitizeFileName(fileName);

        // Validate file extension
        if (!IsAllowedExtension(sanitizedFileName))
        {
            return CreateErrorResult("File extension is not allowed");
        }

        // Validate file type by magic bytes
        if (_options.ValidateFileType && !IsValidFileType(fileStream, sanitizedFileName))
        {
            return CreateErrorResult("File type does not match the file extension or is not a valid file");
        }

        // Validate against expected file type
        if (expectedFileType.HasValue && expectedFileType != FileType.Any &&
            !IsExpectedFileType(fileStream, sanitizedFileName, expectedFileType.Value))
        {
            return CreateErrorResult($"File does not match expected file type: {expectedFileType}");
        }

        var targetDirectory = GetTargetDirectory(subfolder);
        EnsureDirectoryExists(targetDirectory);

        var fileId = Guid.CreateVersion7();
        var uniqueFileName = $"{fileId}{Path.GetExtension(sanitizedFileName)}";
        var fullPath = Path.Combine(targetDirectory, uniqueFileName);
        var relativePath = Path.GetRelativePath(_options.RootPath, fullPath).Replace('\\', '/');

        // Save file to disk
        await SaveStreamToFile(fileStream, fullPath, cancellationToken);

        var contentType = GetContentType(Path.GetExtension(sanitizedFileName));

        // Save file info to database
        var uploadedFile = new UploadedFile(
            fileId,
            fileName,
            relativePath,
            contentType,
            fileStream.Length,
            subfolder
        );

        await _db.UploadedFiles.AddAsync(uploadedFile, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new FileUploadResult
        {
            FileId = fileId,
            Success = true,
            FilePath = relativePath,
            FileName = uniqueFileName,
            OriginalFileName = fileName,
            FileSize = fileStream.Length
        };
    }

    private async Task SaveStreamToFile(Stream sourceStream, string targetPath, CancellationToken cancellationToken)
    {
        using var fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 81920, useAsync: true);
        await sourceStream.CopyToAsync(fileStream, cancellationToken);
        await fileStream.FlushAsync(cancellationToken);
    }

    private (bool IsValid, string ErrorMessage) ValidateFileInput(Stream fileStream, string fileName)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            return (false, "File stream is empty or null");
        }

        if (_options.MaxFileSize > 0 && fileStream.Length > _options.MaxFileSize)
        {
            return (false, $"File size exceeds maximum allowed size of {_options.MaxFileSize} bytes");
        }

        var sanitizedFileName = SanitizeFileName(fileName);
        if (string.IsNullOrEmpty(sanitizedFileName))
        {
            return (false, "Invalid file name");
        }

        return (true, string.Empty);
    }

    private FileUploadResult CreateErrorResult(string errorMessage)
    {
        return new FileUploadResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }

    private void EnsureDirectoryExists(string directoryPath)
    {
        if (_directoryCache.ContainsKey(directoryPath))
            return;

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        _directoryCache.TryAdd(directoryPath, true);
    }

    private string GetTargetDirectory(string? subfolder)
    {
        if (string.IsNullOrEmpty(subfolder))
        {
            return _options.RootPath;
        }

        var sanitizedSubfolder = SanitizeFileName(subfolder);
        return Path.Combine(_options.RootPath, sanitizedSubfolder);
    }

    private string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return string.Empty;

        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
        return sanitized.Trim();
    }

    private bool IsAllowedExtension(string fileName)
    {
        if (_options.AllowedExtensions == null || _options.AllowedExtensions.Length == 0)
        {
            return true;
        }

        var extension = Path.GetExtension(fileName);
        return _options.AllowedExtensions.Any(ext => string.Equals(ext, extension, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsValidFileType(Stream fileStream, string fileName)
    {
        if (fileStream == null || fileStream.Length == 0)
            return false;

        if (!TryReadFromStream(fileStream, buffer =>
        {
            if (buffer.Length < 2)
                return false;

            var fileExtension = Path.GetExtension(fileName);
            var detectedType = GetFileTypeFromMagicBytes(buffer);

            if (string.IsNullOrEmpty(detectedType))
            {
                return IsAllowedExtension(fileName);
            }

            return IsFileTypeMatchingExtension(detectedType, fileExtension);
        }, out bool isValid))
        {
            return false;
        }

        return isValid;
    }

    private string GetFileTypeFromMagicBytes(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 2) return string.Empty;

        // Image formats - optimized with ReadOnlySpan
        if (buffer.Length >= 8 && buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
            return "png";

        if (buffer.Length >= 3 && buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
            return "jpeg";

        if (buffer.Length >= 6 && buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46 &&
            buffer[3] == 0x38 && (buffer[4] == 0x37 || buffer[4] == 0x39) && buffer[5] == 0x61)
            return "gif";

        if (buffer.Length >= 2 && buffer[0] == 0x42 && buffer[1] == 0x4D)
            return "bmp";

        if (buffer.Length >= 4 && buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46)
            return "webp";

        // Document formats
        if (buffer.Length >= 4 && buffer[0] == 0x25 && buffer[1] == 0x50 && buffer[2] == 0x44 && buffer[3] == 0x46)
            return "pdf";

        if (buffer.Length >= 4 && buffer[0] == 0x50 && buffer[1] == 0x4B && buffer[2] == 0x03 && buffer[3] == 0x04)
            return "office";

        if (buffer.Length >= 8 && buffer[0] == 0xD0 && buffer[1] == 0xCF && buffer[2] == 0x11 && buffer[3] == 0xE0)
            return "ole";

        // Archive formats
        if (buffer.Length >= 2 && buffer[0] == 0x50 && buffer[1] == 0x4B)
            return "zip";

        if (buffer.Length >= 7 && buffer[0] == 0x52 && buffer[1] == 0x61 && buffer[2] == 0x72 &&
            buffer[3] == 0x21 && buffer[4] == 0x1A && buffer[5] == 0x07 && buffer[6] == 0x00)
            return "rar";

        return string.Empty;
    }

    private bool IsFileTypeMatchingExtension(string detectedType, string fileExtension)
    {
        return detectedType switch
        {
            "png" => string.Equals(fileExtension, ".png", StringComparison.OrdinalIgnoreCase),
            "jpeg" => string.Equals(fileExtension, ".jpg", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(fileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase),
            "gif" => string.Equals(fileExtension, ".gif", StringComparison.OrdinalIgnoreCase),
            "bmp" => string.Equals(fileExtension, ".bmp", StringComparison.OrdinalIgnoreCase),
            "webp" => string.Equals(fileExtension, ".webp", StringComparison.OrdinalIgnoreCase),
            "pdf" => string.Equals(fileExtension, ".pdf", StringComparison.OrdinalIgnoreCase),
            "office" => string.Equals(fileExtension, ".docx", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(fileExtension, ".xlsx", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(fileExtension, ".pptx", StringComparison.OrdinalIgnoreCase),
            "ole" => string.Equals(fileExtension, ".doc", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(fileExtension, ".xls", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(fileExtension, ".ppt", StringComparison.OrdinalIgnoreCase),
            "zip" => string.Equals(fileExtension, ".zip", StringComparison.OrdinalIgnoreCase),
            "rar" => string.Equals(fileExtension, ".rar", StringComparison.OrdinalIgnoreCase),
            _ => true
        };
    }

    private bool IsExpectedFileType(Stream fileStream, string fileName, FileType expectedFileType)
    {
        if (!TryReadFromStream(fileStream, buffer =>
        {
            if (buffer.Length < 2)
                return false;

            var detectedType = GetFileTypeFromMagicBytes(buffer);
            var fileExtension = Path.GetExtension(fileName);

            return expectedFileType switch
            {
                FileType.Any => true,
                FileType.Image => IsImageFile(detectedType, fileExtension),
                FileType.Document => IsDocumentFile(detectedType, fileExtension),
                FileType.Archive => IsArchiveFile(detectedType, fileExtension),
                FileType.Video => _videoExtensions.Contains(fileExtension),
                FileType.Audio => _audioExtensions.Contains(fileExtension),
                FileType.Text => _textExtensions.Contains(fileExtension),
                _ => true
            };
        }, out bool isExpectedType))
        {
            return false;
        }

        return isExpectedType;
    }

    private bool IsImageFile(string detectedType, string fileExtension)
    {
        var imageTypes = new[] { "png", "jpeg", "gif", "bmp", "webp" };
        return imageTypes.Contains(detectedType) || _imageExtensions.Contains(fileExtension);
    }

    private bool IsDocumentFile(string detectedType, string fileExtension)
    {
        var documentTypes = new[] { "pdf", "office", "ole" };
        return documentTypes.Contains(detectedType) || _documentExtensions.Contains(fileExtension);
    }

    private bool IsArchiveFile(string detectedType, string fileExtension)
    {
        var archiveTypes = new[] { "zip", "rar" };
        return archiveTypes.Contains(detectedType) || _archiveExtensions.Contains(fileExtension);
    }

    private string GetContentType(string extension)
    {
        return _contentTypeMappings.TryGetValue(extension.ToLowerInvariant(), out var contentType)
            ? contentType
            : "application/octet-stream";
    }
}
