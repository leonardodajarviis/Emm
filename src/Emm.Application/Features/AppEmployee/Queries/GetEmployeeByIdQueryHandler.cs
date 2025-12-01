using Emm.Application.Features.AppEmployee.Dtos;
using Emm.Domain.Entities.Organization;
using LazyNet.Symphony.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppEmployee.Queries;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeResponse>>
{
    private readonly IQueryContext _queryContext;

    public GetEmployeeByIdQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<EmployeeResponse>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _queryContext.Query<Employee>()
            .Where(x => x.Id == request.Id)
            .Select(x => new EmployeeResponse
            {
                Id = x.Id,
                Code = x.Code,
                DisplayName = x.DisplayName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                OrganizationUnitId = x.OrganizationUnitId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                
                OrganizationUnit = x.OrganizationUnitId != null ? 
                    _queryContext.Query<OrganizationUnit>()
                        .Where(ou => ou.Id == x.OrganizationUnitId)
                        .Select(ou => new OrganizationUnitResponse
                        {
                            Id = ou.Id,
                            Code = ou.Code,
                            Name = ou.Name
                        }).FirstOrDefault() : null
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (employee == null)
        {
            return Result<EmployeeResponse>.Failure(ErrorType.NotFound, "Employee not found.");
        }

        return Result<EmployeeResponse>.Success(employee);
    }
}
