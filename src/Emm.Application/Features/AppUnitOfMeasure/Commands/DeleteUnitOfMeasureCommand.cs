using Emm.Application.Common;

namespace Emm.Application.Features.AppUnitOfMeasure.Commands;

public record DeleteUnitOfMeasureCommand(Guid Id) : IRequest<Result<object>>;
