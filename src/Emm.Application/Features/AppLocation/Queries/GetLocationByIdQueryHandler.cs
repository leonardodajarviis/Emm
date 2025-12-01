using Emm.Application.Abstractions;
using Emm.Application.Features.AppLocation.Dtos;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppLocation.Queries;

public class GetLocationByIdQueryHandler : IRequestHandler<GetLocationByIdQuery, Result<object>>
{
    private readonly IQueryContext _queryContext;

    public GetLocationByIdQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<object>> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
    {
        var location = await _queryContext.Query<Location>()
            .Where(x => x.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (location == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Location not found");
        }

        return Result<object>.Success(location);
    }
}
