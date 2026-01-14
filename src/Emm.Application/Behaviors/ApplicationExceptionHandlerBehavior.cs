using System.Reflection;
using Emm.Application.ErrorCodes;
using Emm.Domain.Exceptions;
using LazyNet.Symphony.Core;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Behaviors
{
    public class ApplicationExceptionHandlerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public ApplicationExceptionHandlerBehavior()
        {
        }

        public async Task<TResponse> Handle(TRequest request, PipelineNext<TResponse> next, CancellationToken cancellationToken = default)
        {
            try
            {
                return await next();
            }
            catch (DbUpdateException ex)
            {
                return HandleDbUpdateExceptionForResult(ex);
            }
            catch (DomainException ex)
            {
                return HandleDomainExceptionForResult(ex);
            }
        }

        private static bool IsResultType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>);
        }

        private static TResponse HandleDomainExceptionForResult(DomainException ex)
        {
            var resultType = typeof(TResponse);
            var failureMethod = resultType.GetMethod("Failure", BindingFlags.Public | BindingFlags.Static) ?? throw new InvalidOperationException($"Could not find Failure method on {resultType.Name}");

            var result = failureMethod.Invoke(null, [ErrorType.Validation, ex.Message, ex.Rule]);
            return (TResponse)result!;
        }

        private static TResponse HandleDbUpdateExceptionForResult(DbUpdateException ex)
        {
            var resultType = typeof(TResponse);
            var failureMethod = resultType.GetMethod("Failure", BindingFlags.Public | BindingFlags.Static);

            if (failureMethod == null)
            {
                throw new InvalidOperationException($"Could not find Failure method on {resultType.Name}");
            }

            var errorType = DetermineDbErrorType(ex);
            var errorMessage = GetDbUpdateErrorMessage(ex);
            var errorCode = GetDbUpdateErrorCode(ex);

            var result = failureMethod.Invoke(null, [errorType, errorMessage, errorCode]);
            return (TResponse)result!;
        }

        private static ErrorType DetermineDbErrorType(DbUpdateException ex)
        {
            var innerException = ex.InnerException?.Message?.ToUpperInvariant() ?? string.Empty;

            // Kiểm tra các lỗi database phổ biến
            if (innerException.Contains("UNIQUE") ||
                innerException.Contains("DUPLICATE") ||
                innerException.Contains("ALREADY EXISTS"))
            {
                return ErrorType.Conflict;
            }

            if (innerException.Contains("FOREIGN KEY") ||
                innerException.Contains("REFERENCE"))
            {
                return ErrorType.Conflict;
            }

            if (innerException.Contains("NULL") ||
                innerException.Contains("REQUIRED"))
            {
                return ErrorType.Validation;
            }

            return ErrorType.Internal;
        }

        private static string GetDbUpdateErrorMessage(DbUpdateException ex)
        {
            var innerException = ex.InnerException?.Message ?? ex.Message;
            var innerMsg = innerException.ToUpperInvariant();

            // Tạo message thân thiện với người dùng
            if (innerMsg.Contains("UNIQUE") || innerMsg.Contains("DUPLICATE"))
            {
                return "A record with the same value already exists.";
            }

            if (innerMsg.Contains("FOREIGN KEY"))
            {
                return "Cannot perform this operation due to related data constraints.";
            }

            if (innerMsg.Contains("NULL") || innerMsg.Contains("REQUIRED"))
            {
                return "Required field is missing or invalid.";
            }

            return "An error occurred while updating the database.";
        }

        private static string GetDbUpdateErrorCode(DbUpdateException ex)
        {
            var innerException = ex.InnerException?.Message?.ToUpperInvariant() ?? string.Empty;

            if (innerException.Contains("UNIQUE") || innerException.Contains("DUPLICATE"))
            {
                return DatabaseErrorCodes.UniqueConstraint;
            }

            if (innerException.Contains("FOREIGN KEY"))
            {
                return DatabaseErrorCodes.ForeignKeyViolation;
            }

            if (innerException.Contains("NULL") || innerException.Contains("REQUIRED"))
            {
                return ValidationErrorCodes.FieldRequired;
            }

            return GeneralErrorCodes.UnknownError;
        }
    }
}
