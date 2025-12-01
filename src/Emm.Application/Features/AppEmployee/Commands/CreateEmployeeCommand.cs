using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppEmployee.Commands;

public record CreateEmployeeCommand(
    string DisplayName,
    string FirstName,
    string? LastName,
    long? OrganizationUnitId
) : IRequest<Result<object>>;
