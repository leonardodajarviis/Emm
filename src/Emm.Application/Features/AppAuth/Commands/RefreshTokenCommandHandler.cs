using Emm.Application.Abstractions;
using Emm.Application.Features.AppAuth.Dtos;
using Emm.Domain.Repositories;

namespace Emm.Application.Features.AppAuth.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<UserAuthResponse>>
{
    private readonly IJwtService _jwtService;
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IJwtService jwtService,
        IUserSessionRepository userSessionRepository,
        IUnitOfWork unitOfWork)
    {
        _jwtService = jwtService;
        _userSessionRepository = userSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserAuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Validate refresh token

        if (!_jwtService.ValidateRefreshToken(request.RefreshToken, out _, out var jti))
        {
            return Result<UserAuthResponse>.Failure(ErrorType.Unauthorized, "Invalid refresh token.");
        }

        // Get session by refresh token JTI
        var session = await _userSessionRepository.GetByRefreshTokenJtiAsync(jti, cancellationToken);

        if (session == null)
        {
            return Result<UserAuthResponse>.Failure(ErrorType.Unauthorized, "Session not found.");
        }

        if (session.IsRevoked)
        {
            return Result<UserAuthResponse>.Failure(ErrorType.Unauthorized, "Session has been revoked.");
        }

        if (session.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            return Result<UserAuthResponse>.Failure(ErrorType.Unauthorized, "Refresh token has expired.");
        }

        // Generate new tokens
        var tokens = _jwtService.RefreshTokens(request.RefreshToken);

        // Update session with new tokens
        session.UpdateTokens(
            accessTokenJti: tokens.AccessTokenJti,
            refreshTokenJti: tokens.RefreshTokenJti,
            accessTokenExpiresAt: DateTimeOffset.FromUnixTimeSeconds(tokens.AccessTokenExpiresAt).UtcDateTime,
            refreshTokenExpiresAt: DateTimeOffset.FromUnixTimeSeconds(tokens.RefreshTokenExpiresAt).UtcDateTime
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new UserAuthResponse(
            AccessToken: tokens.AccessToken,
            RefreshToken: tokens.RefreshToken,
            AccessTokenExpiresAt: tokens.AccessTokenExpiresAt,
            RefreshTokenExpiresAt: tokens.RefreshTokenExpiresAt,
            User: new UserAuthInfo(
                UserId: session.UserId,
                Username: session.User.Username,
                DisplayName: session.User.DisplayName,
                Email: session.User.Email
            ));

        return Result<UserAuthResponse>.Success(response);
    }
}
