using Emm.Application.Features.AppRole.Dtos;
using Emm.Domain.Entities.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppRole.Queries;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<RoleResponse>>
{
    private readonly IQueryContext _queryContext;

    public GetRoleByIdQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<RoleResponse>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _queryContext.Query<Role>()
            .Where(x => x.Id == request.Id)
            .Select(x => new RoleResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                IsSystemRole = x.IsSystemRole,
                IsActive = x.IsActive,
                CreatedAt = x.Audit.CreatedAt,
                ModifiedAt = x.Audit.ModifiedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (role == null)
        {
            return Result<RoleResponse>.Failure(ErrorType.NotFound, "Role not found.");
        }

        return Result<RoleResponse>.Success(role);
    }
}
