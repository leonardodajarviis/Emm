using Emm.Application.Features.AppOrganizationUnitLevel.Dtos;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOrganizationUnitLevel.Queries;

public class GetOrganizationUnitLevelsQueryHandler : IRequestHandler<GetOrganizationUnitLevelsQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;

    public GetOrganizationUnitLevelsQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<PagedResult>> Handle(GetOrganizationUnitLevelsQuery request, CancellationToken cancellationToken)
    {
        var query = _queryContext.Query<OrganizationUnitLevel>();

        var total = query.Count();

        var result = await query
            .OrderBy(x => x.Level)
            .Skip((request.QueryRequest.Page - 1) * request.QueryRequest.PageSize)
            .Take(request.QueryRequest.PageSize)
            .Select(x => new OrganizationUnitLevelResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Level = x.Level,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult>.Success(request.QueryRequest.AsPagedResult(total, result));
    }
}
