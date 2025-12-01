using Emm.Application.Features.AppUser.Dtos;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Queries;

public record GetUserByIdQuery(
    long Id
) : IRequest<Result<UserResponse>>;
