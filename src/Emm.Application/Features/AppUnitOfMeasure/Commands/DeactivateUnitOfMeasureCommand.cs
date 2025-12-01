using Emm.Application.Common;

namespace Emm.Application.Features.AppUnitOfMeasure.Commands;

public record DeactivateUnitOfMeasureCommand(long Id) : IRequest<Result<object>>;
