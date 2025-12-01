using Emm.Application.Abstractions;
using Emm.Domain.Repositories;

namespace Emm.Application.Features.AppAuth.Commands;

public class UserLogoutCommandHandler : IRequestHandler<UserLogoutCommand, Result<bool>>
{
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly IJwtService _jwtService;

    public UserLogoutCommandHandler(
        IUserSessionRepository userSessionRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService,
        IJwtService jwtService)
    {
        _userSessionRepository = userSessionRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
        _jwtService = jwtService;
    }

    public async Task<Result<bool>> Handle(UserLogoutCommand request, CancellationToken cancellationToken)
    {
        var token = _userContextService.GetAccessToken();

        if (string.IsNullOrEmpty(token))
        {
            return Result<bool>.Failure(ErrorType.Unauthorized, "No access token found.");
        }

        if (!_jwtService.ValidateAccessToken(token, out var userId, out var jti))
        {
            return Result<bool>.Failure(ErrorType.Unauthorized, "Invalid access token.");
        }

        var session = await _userSessionRepository.GetByAccessTokenJtiAsync(jti, cancellationToken);

        if (session == null)
        {
            return Result<bool>.Failure(ErrorType.NotFound, "Session not found.");
        }

        if (session.IsRevoked)
        {
            return Result<bool>.Failure(ErrorType.Invalid, "Session already revoked.");
        }

        session.Revoke();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
