using Emm.Application.Features.AppUser.Dtos;
using Emm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppUser.Queries;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;

    public GetUsersQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<PagedResult>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _queryContext.Query<User>();

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.QueryRequest.Page - 1) * request.QueryRequest.PageSize)
            .Take(request.QueryRequest.PageSize)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                Username = x.Username,
                DisplayName = x.DisplayName,
                Email = x.Email,
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
