using Emm.Application.Common;

namespace Emm.Application.Features.AppUnitOfMeasure.Commands;

public record ActivateUnitOfMeasureCommand(long Id) : IRequest<Result<object>>;
