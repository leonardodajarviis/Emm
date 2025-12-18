using Emm.Application.Features.AppOperationShift.Dtos;

namespace Emm.Application.Features.AppOperationShift.Queries;

public record GetOperationShiftByIdQuery(
    Guid Id
) : IRequest<Result<OperationShiftResponse>>;
