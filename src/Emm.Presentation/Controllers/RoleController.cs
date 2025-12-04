using Emm.Domain.Entities.Authorization;
using Emm.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/roles")]
public class RoleController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RoleController(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetAllAsync(includeInactive, cancellationToken);
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(id, cancellationToken);
        if (role == null)
            return NotFound();

        return Ok(new
        {
            role.Id,
            role.Code,
            role.Name,
            role.Description,
            role.IsSystemRole,
            role.IsActive,
            Permissions = role.RolePermissions.Select(rp => new
            {
                rp.Permission.Id,
                rp.Permission.Code,
                rp.Permission.DisplayName,
                rp.Permission.Resource,
                rp.Permission.Action
            })
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var exists = await _roleRepository.ExistsAsync(request.Code, cancellationToken);
        if (exists)
            return BadRequest("Role code already exists");

        var role = new Role(request.Code, request.Name, request.Description);

        await _roleRepository.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (role == null)
            return NotFound();

        role.Update(request.Name, request.Description);
        _roleRepository.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(role);
    }

    [HttpPost("{id}/permissions")]
    public async Task<IActionResult> AddPermission(long id, [FromBody] AddPermissionToRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(id, cancellationToken);
        if (role == null)
            return NotFound("Role not found");

        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
        if (permission == null)
            return NotFound("Permission not found");

        role.AddPermission(request.PermissionId);
        _roleRepository.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    [HttpDelete("{id}/permissions/{permissionId}")]
    public async Task<IActionResult> RemovePermission(long id, long permissionId, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(id, cancellationToken);
        if (role == null)
            return NotFound();

        role.RemovePermission(permissionId);
        _roleRepository.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("{id}/activate")]
    public async Task<IActionResult> Activate(long id, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (role == null)
            return NotFound();

        role.Activate();
        _roleRepository.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(long id, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (role == null)
            return NotFound();

        role.Deactivate();
        _roleRepository.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (role == null)
            return NotFound();

        if (role.IsSystemRole)
            return BadRequest("Cannot delete system role");

        _roleRepository.Delete(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("users/{userId}/roles")]
    public async Task<IActionResult> AssignRoleToUser(long userId, [FromBody] AssignRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
            return NotFound("Role not found");

        var exists = await _userRoleRepository.ExistsAsync(userId, request.RoleId, cancellationToken);
        if (exists)
            return BadRequest("User already has this role");

        var userIdClaim = User.FindFirst("userId");
        long? assignedBy = userIdClaim != null && long.TryParse(userIdClaim.Value, out var id) ? id : null;

        var userRole = new UserRole(userId, request.RoleId, assignedBy);
        await _userRoleRepository.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    [HttpDelete("users/{userId}/roles/{roleId}")]
    public async Task<IActionResult> RemoveRoleFromUser(long userId, long roleId, CancellationToken cancellationToken)
    {
        var userRole = await _userRoleRepository.GetAsync(userId, roleId, cancellationToken);
        if (userRole == null)
            return NotFound();

        await _userRoleRepository.DeleteAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}

public record CreateRoleRequest(string Code, string Name, string? Description);
public record UpdateRoleRequest(string Name, string? Description);
public record AddPermissionToRoleRequest(long PermissionId);
public record AssignRoleRequest(long RoleId);
