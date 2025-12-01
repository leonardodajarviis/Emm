using Emm.Application.Abstractions;
using Emm.Application.Features.AppOrganizationUnit.Dtos;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOrganizationUnit.Queries;

public class GetOrganizationUnitByIdQueryHandler : IRequestHandler<GetOrganizationUnitByIdQuery, Result<object>>
{
    private readonly IQueryContext _queryContext;

    public GetOrganizationUnitByIdQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<object>> Handle(GetOrganizationUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var organizationUnit = await _queryContext.Query<OrganizationUnit>()
            .Where(x => x.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (organizationUnit == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Organization unit not found");
        }

        return Result<object>.Success(organizationUnit);
    }
}
