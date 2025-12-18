using Emm.Application.Features.AppUser.Dtos;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Queries;

public record GetUserByIdQuery(
    Guid Id
) : IRequest<Result<UserResponse>>;
