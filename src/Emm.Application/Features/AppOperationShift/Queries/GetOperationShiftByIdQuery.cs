using Emm.Application.Features.AppOperationShift.Dtos;

namespace Emm.Application.Features.AppOperationShift.Queries;

public record GetOperationShiftByIdQuery(
    long Id
) : IRequest<Result<OperationShiftResponse>>;
