using Emm.Application.Abstractions;
using Emm.Application.Features.AppOrganizationUnit.Dtos;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOrganizationUnit.Queries;

public class GetOrganizationUnitsQueryHandler : IRequestHandler<GetOrganizationUnitsQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;

    public GetOrganizationUnitsQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<PagedResult>> Handle(GetOrganizationUnitsQuery request, CancellationToken cancellationToken)
    {
        var query = _queryContext.Query<OrganizationUnit>();

        var total = query.Count();

        var result = await query
            .OrderBy(x => x.Name)
            .Skip((request.QueryRequest.Page - 1) * request.QueryRequest.PageSize)
            .Take(request.QueryRequest.PageSize)
            .Select(x => new OrganizationUnitResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                ParentId = x.ParentId,
                OrganizationUnitLevelId = x.OrganizationUnitLevelId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult>.Success(request.QueryRequest.AsPagedResult(total, result));
    }
}
