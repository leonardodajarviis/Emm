using Emm.Application.Abstractions;
using Emm.Application.Features.AppOrganizationUnitLevel.Dtos;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOrganizationUnitLevel.Queries;

public class GetOrganizationUnitLevelByIdQueryHandler : IRequestHandler<GetOrganizationUnitLevelByIdQuery, Result<object>>
{
    private readonly IQueryContext _queryContext;

    public GetOrganizationUnitLevelByIdQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<object>> Handle(GetOrganizationUnitLevelByIdQuery request, CancellationToken cancellationToken)
    {
        var organizationUnitLevel = await _queryContext.Query<OrganizationUnitLevel>()
            .Where(x => x.Id == request.Id)
            .Select(x => new OrganizationUnitLevelResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Level = x.Level,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (organizationUnitLevel == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Organization unit level not found");
        }

        return Result<object>.Success(organizationUnitLevel);
    }
}
