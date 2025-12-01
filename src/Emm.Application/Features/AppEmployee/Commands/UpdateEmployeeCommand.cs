using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppEmployee.Commands;

public record UpdateEmployeeCommand(
    long Id,
    string DisplayName,
    string FirstName,
    string? LastName
) : IRequest<Result<object>>;
