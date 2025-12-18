using System.Security.Claims;
using Emm.Application.Abstractions;
using Emm.Application.Features.AppAuth.Dtos;
using Emm.Domain.Entities;
using Emm.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Emm.Application.Features.AppAuth.Commands;

public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, Result<UserAuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticationSettings _authSettings;

    public UserLoginCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        IPasswordHasher passwordHasher,
        IUserSessionRepository userSessionRepository,
        IUnitOfWork unitOfWork,
        IAuthenticationSettings authSettings)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _userSessionRepository = userSessionRepository;
        _unitOfWork = unitOfWork;
        _authSettings = authSettings;
    }

    public async Task<Result<UserAuthResponse>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (user is null)
        {
            return Result<UserAuthResponse>.Unauthorized("Invalid username or password.");
        }

        if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            return Result<UserAuthResponse>.Unauthorized("Invalid username or password.");
        }

        if (!user.IsActive)
        {
            return Result<UserAuthResponse>.Unauthorized("User account is inactive.");
        }

        // If single-device mode, revoke all existing sessions first
        if (!_authSettings.IsMultiDeviceLoginAllowed)
        {
            await _userSessionRepository.RevokeAllUserSessionsAsync(user.Id, cancellationToken);
        }

        var tokens = _jwtService.GenerateTokens(user.Id, user.Username, identity =>
        {
            identity.AddClaim(new Claim("organizationUnitId", user.OrganizationUnitId?.ToString() ?? "no-organization-unit"));
            // Add other claims as needed
        });

        // Lưu UserSession vào database
        var userSession = new UserSession(
            userId: user.Id,
            accessTokenJti: tokens.AccessTokenJti,
            refreshTokenJti: tokens.RefreshTokenJti,
            accessTokenExpiresAt: DateTimeOffset.FromUnixTimeSeconds(tokens.AccessTokenExpiresAt).UtcDateTime,
            refreshTokenExpiresAt: DateTimeOffset.FromUnixTimeSeconds(tokens.RefreshTokenExpiresAt).UtcDateTime,
            ipAddress: request.IpAddress,
            userAgent: request.UserAgent
        );

        await _userSessionRepository.AddAsync(userSession, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new UserAuthResponse(
            AccessToken: tokens.AccessToken,
            RefreshToken: tokens.RefreshToken,
            AccessTokenExpiresAt: tokens.AccessTokenExpiresAt,
            RefreshTokenExpiresAt: tokens.RefreshTokenExpiresAt,
            User: new UserAuthInfo(
                UserId: user.Id,
                Username: user.Username,
                DisplayName: user.DisplayName,
                Email: user.Email
            ));


        return Result<UserAuthResponse>.Success(response);
    }
}
