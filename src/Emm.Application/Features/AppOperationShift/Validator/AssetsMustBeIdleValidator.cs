using Emm.Application.Abstractions;
using Emm.Application.Features.AppOperationShift.Commands;

namespace Emm.Application.Features.AppOperationShift.Validator;

public class AssetsMustBeIdleValidator : ICommandValidator<CreateOperationShiftCommand>
{
    public Task<Result?> ValidateAsync(CreateOperationShiftCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

}