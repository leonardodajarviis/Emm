namespace Emm.Application.Common.ErrorCodes;

/// <summary>
/// Database error codes
/// </summary>
public static class DatabaseErrorCodes
{
    public const string UniqueConstraint = "DB_UNIQUE_CONSTRAINT";
    public const string ForeignKeyViolation = "DB_FOREIGN_KEY_VIOLATION";
    public const string ConnectionFailed = "DB_CONNECTION_FAILED";
    public const string Timeout = "DB_TIMEOUT";
    public const string ConcurrencyConflict = "DB_CONCURRENCY_CONFLICT";
    public const string DeadlockDetected = "DB_DEADLOCK_DETECTED";
    public const string TransactionFailed = "DB_TRANSACTION_FAILED";
}
