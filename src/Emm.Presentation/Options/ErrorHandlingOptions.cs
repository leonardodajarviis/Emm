namespace Emm.Presentation.Options;

public class ErrorHandlingOptions
{
    public const string SectionName = "ErrorHandling";

    /// <summary>
    /// Include detailed exception messages in error response
    /// Should be true in Development, false in Production
    /// </summary>
    public bool IncludeExceptionDetails { get; set; } = false;

    /// <summary>
    /// Maximum depth of inner exceptions to include
    /// Default: 3
    /// </summary>
    public int MaxExceptionDepth { get; set; } = 3;

    /// <summary>
    /// Include stack trace in error response
    /// Should be false in Production for security
    /// </summary>
    public bool IncludeStackTrace { get; set; } = false;
}
