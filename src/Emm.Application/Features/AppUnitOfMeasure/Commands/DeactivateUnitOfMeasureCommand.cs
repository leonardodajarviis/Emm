using Emm.Application.Common;

namespace Emm.Application.Features.AppUnitOfMeasure.Commands;

public record DeactivateUnitOfMeasureCommand(Guid Id) : IRequest<Result<object>>;
