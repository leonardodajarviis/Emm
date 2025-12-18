using Emm.Application.Features.AppRole.Dtos;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Queries;

public record GetRoleByIdQuery(
    Guid Id
) : IRequest<Result<RoleResponse>>;
