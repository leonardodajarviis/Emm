using Emm.Application.Abstractions;
using Emm.Application.Features.AppLocation.Dtos;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppLocation.Queries;

public class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;

    public GetLocationsQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<PagedResult>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
    {
        var query = _queryContext.Query<Location>();

        var total = query.Count();

        var result = await query
            .OrderBy(x => x.Name)
            .Skip((request.QueryRequest.Page - 1) * request.QueryRequest.PageSize)
            .Take(request.QueryRequest.PageSize)
            .Select(x => new LocationResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                OrganizationUnitId = x.OrganizationUnitId,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult>.Success(request.QueryRequest.AsPagedResult(total, result));
    }
}
