using Emm.Application.Abstractions;
using Emm.Domain.Entities.Authorization;
using Emm.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/user-permissions")]
public class UserPermissionController : ControllerBase
{
    private readonly IUserPermissionRepository _userPermissionRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUnitOfWork _unitOfWork;

    public UserPermissionController(
        IUserPermissionRepository userPermissionRepository,
        IPermissionRepository permissionRepository,
        IAuthorizationService authorizationService,
        IUnitOfWork unitOfWork)
    {
        _userPermissionRepository = userPermissionRepository;
        _permissionRepository = permissionRepository;
        _authorizationService = authorizationService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserPermissions(long userId, CancellationToken cancellationToken)
    {
        var permissions = await _authorizationService.GetUserPermissionsAsync(userId, cancellationToken);
        var roles = await _authorizationService.GetUserRolesAsync(userId, cancellationToken);

        return Ok(new
        {
            Permissions = permissions,
            Roles = roles
        });
    }

    [HttpPost("users/{userId}/permissions")]
    public async Task<IActionResult> GrantPermission(
        long userId,
        [FromBody] GrantPermissionRequest request,
        CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
        if (permission == null)
            return NotFound("Permission not found");

        var exists = await _userPermissionRepository.ExistsAsync(userId, request.PermissionId, cancellationToken);
        if (exists)
        {
            // Update existing
            var existing = await _userPermissionRepository.GetAsync(userId, request.PermissionId, cancellationToken);
            if (existing != null)
            {
                await _userPermissionRepository.DeleteAsync(existing, cancellationToken);
            }
        }

        var userIdClaim = User.FindFirst("userId");
        long? assignedBy = userIdClaim != null && long.TryParse(userIdClaim.Value, out var id) ? id : null;

        var userPermission = new UserPermission(
            userId,
            request.PermissionId,
            request.IsGranted,
            assignedBy,
            request.Reason);

        await _userPermissionRepository.AddAsync(userPermission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    [HttpDelete("users/{userId}/permissions/{permissionId}")]
    public async Task<IActionResult> RevokePermission(long userId, long permissionId, CancellationToken cancellationToken)
    {
        var userPermission = await _userPermissionRepository.GetAsync(userId, permissionId, cancellationToken);
        if (userPermission == null)
            return NotFound();

        await _userPermissionRepository.DeleteAsync(userPermission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("check")]
    public async Task<IActionResult> CheckPermission([FromBody] CheckPermissionRequest request, CancellationToken cancellationToken)
    {
        var hasPermission = await _authorizationService.HasPermissionAsync(request.UserId, request.PermissionCode, cancellationToken);
        return Ok(new { HasPermission = hasPermission });
    }
}

public record GrantPermissionRequest(long PermissionId, bool IsGranted = true, string? Reason = null);
public record CheckPermissionRequest(long UserId, string PermissionCode);
