using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppEmployee.Commands;

public record DeleteEmployeeCommand(
    long Id
) : IRequest<Result<object>>;
