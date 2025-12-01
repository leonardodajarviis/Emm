using Emm.Application.Abstractions;
using Emm.Application.Features.AppUser.Dtos;
using Emm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppUser.Queries;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserResponse>>
{
    private readonly IQueryContext _queryContext;
    private readonly IUserContextService _userContextService;

    public GetUserByIdQueryHandler(IQueryContext queryContext, IUserContextService userContextService)
    {
        _queryContext = queryContext;
        _userContextService = userContextService;
    }

    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _queryContext.Query<User>()
            .Where(x => x.Id == request.Id)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                Username = x.Username,
                DisplayName = x.DisplayName,
                Email = x.Email,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return Result<UserResponse>.Failure(ErrorType.NotFound, "User not found.");
        }

        return Result<UserResponse>.Success(user);
    }
}
