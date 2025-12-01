using Emm.Application.Features.AppEmployee.Dtos;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppEmployee.Queries;

public record GetEmployeeByIdQuery(
    long Id
) : IRequest<Result<EmployeeResponse>>;
