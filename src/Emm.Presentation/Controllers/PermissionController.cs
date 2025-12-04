using Emm.Domain.Entities.Authorization;
using Emm.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/permissions")]
public class PermissionController : ControllerBase
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PermissionController(
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);
        return Ok(permissions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(id, cancellationToken);
        if (permission == null)
            return NotFound();

        return Ok(permission);
    }

    [HttpGet("resource/{resource}")]
    public async Task<IActionResult> GetByResource(string resource, CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetByResourceAsync(resource, cancellationToken);
        return Ok(permissions);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePermissionRequest request, CancellationToken cancellationToken)
    {
        var exists = await _permissionRepository.ExistsAsync($"{request.Resource}.{request.Action}", cancellationToken);
        if (exists)
            return BadRequest("Permission already exists");

        var permission = new Permission(
            request.Resource,
            request.Action,
            request.DisplayName,
            request.Description,
            request.Category);

        await _permissionRepository.AddAsync(permission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = permission.Id }, permission);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdatePermissionRequest request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(id, cancellationToken);
        if (permission == null)
            return NotFound();

        permission.Update(request.DisplayName, request.Description, request.Category);
        _permissionRepository.Update(permission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(permission);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(id, cancellationToken);
        if (permission == null)
            return NotFound();

        _permissionRepository.Delete(permission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}

public record CreatePermissionRequest(
    string Resource,
    string Action,
    string DisplayName,
    string? Description,
    string? Category);

public record UpdatePermissionRequest(
    string DisplayName,
    string? Description,
    string? Category);
