namespace Emm.Application.ErrorCodes;

/// <summary>
/// File storage error codes
/// </summary>
public static class FileErrorCodes
{
    public const string NotFound = "FILE_NOT_FOUND";
    public const string UploadFailed = "FILE_UPLOAD_FAILED";
    public const string DeleteFailed = "FILE_DELETE_FAILED";
    public const string TooLarge = "FILE_TOO_LARGE";
    public const string TypeNotAllowed = "FILE_TYPE_NOT_ALLOWED";
    public const string NameInvalid = "FILE_NAME_INVALID";
    public const string StorageError = "FILE_STORAGE_ERROR";
    public const string EmptyFile = "FILE_EMPTY";
    public const string CorruptedFile = "FILE_CORRUPTED";
}
