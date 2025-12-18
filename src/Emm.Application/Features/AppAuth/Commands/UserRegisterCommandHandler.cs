using System.Security.Claims;
using Emm.Application.Abstractions;
using Emm.Application.Features.AppAuth.Dtos;
using Emm.Domain.Entities;
using Emm.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Emm.Application.Features.AppAuth.Commands;

public class UserRegisterCommandHandler : IRequestHandler<UserRegisterCommand, Result<UserAuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserRegisterCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        IPasswordHasher passwordHasher,
        IUserSessionRepository userSessionRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _userSessionRepository = userSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserAuthResponse>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return Result<UserAuthResponse>.Failure(ErrorType.Validation, "Username is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Result<UserAuthResponse>.Failure(ErrorType.Validation, "Password is required.");
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return Result<UserAuthResponse>.Failure(ErrorType.Validation, "Display name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result<UserAuthResponse>.Failure(ErrorType.Validation, "Email is required.");
        }

        // Check if username already exists
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (existingUser is not null)
        {
            return Result<UserAuthResponse>.Failure(ErrorType.Conflict, "Username already exists.");
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create new user
        var user = new User(
            username: request.Username,
            displayName: request.DisplayName,
            passwordHash: passwordHash,
            email: request.Email
        );

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var tokens = _jwtService.GenerateTokens(user.Id, user.Username, identity =>
        {
            identity.AddClaim(new Claim("organizationUnitId", user.OrganizationUnitId?.ToString() ?? "no-organization-unit"));
        });

        // Create user session
        var userSession = new UserSession(
            userId: user.Id,
            accessTokenJti: tokens.AccessTokenJti,
            refreshTokenJti: tokens.RefreshTokenJti,
            accessTokenExpiresAt: DateTimeOffset.FromUnixTimeSeconds(tokens.AccessTokenExpiresAt).UtcDateTime,
            refreshTokenExpiresAt: DateTimeOffset.FromUnixTimeSeconds(tokens.RefreshTokenExpiresAt).UtcDateTime,
            ipAddress: null,
            userAgent: null
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
