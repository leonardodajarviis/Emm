using Emm.Application.Common;

namespace Emm.Application.Features.AppUnitOfMeasure.Commands;

public record ActivateUnitOfMeasureCommand(Guid Id) : IRequest<Result<object>>;
