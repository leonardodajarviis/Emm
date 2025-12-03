using Emm.Application.Features.AppRole.Dtos;
using Emm.Domain.Entities.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppRole.Queries;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;

    public GetRolesQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<PagedResult>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var query = _queryContext.Query<Role>();

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.QueryRequest.Page - 1) * request.QueryRequest.PageSize)
            .Take(request.QueryRequest.PageSize)
            .Select(x => new RoleResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                IsSystemRole = x.IsSystemRole,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
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
