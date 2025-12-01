using Emm.Application.Features.AppEmployee.Dtos;
using Emm.Domain.Entities.Organization;
using LazyNet.Symphony.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppEmployee.Queries;

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;

    public GetEmployeesQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<PagedResult>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var query = _queryContext.Query<Employee>();

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.QueryRequest.Page - 1) * request.QueryRequest.PageSize)
            .Take(request.QueryRequest.PageSize)
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
                
                // For list view, we don't include organization unit details for performance
                OrganizationUnit = null
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult(
            page: request.QueryRequest.Page,
            pageSize: request.QueryRequest.PageSize,
            totalCount: totalCount,
            results: items.Cast<object>().ToList()
        );

        return Result<PagedResult>.Success(pagedResult);
    }
}
