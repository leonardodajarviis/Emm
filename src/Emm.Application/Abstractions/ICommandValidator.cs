namespace Emm.Application.Abstractions;

public interface ICommandValidator<in TCommand>
{
    Task<Result?> ValidateAsync(TCommand command, CancellationToken cancellationToken = default);
}
