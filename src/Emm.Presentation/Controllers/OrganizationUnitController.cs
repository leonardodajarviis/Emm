using Emm.Application.Features.AppOrganizationUnit.Commands;
using Emm.Application.Features.AppOrganizationUnit.Dtos;
using Emm.Application.Features.AppOrganizationUnit.Queries;
using Emm.Application.Common;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/organization-units")]
public class OrganizationUnitController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganizationUnitController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganizationUnit([FromBody] CreateOrganizationUnit createOrganizationUnit)
    {
        var command = new CreateOrganizationUnitCommand(
            Name: createOrganizationUnit.Name,
            OrganizationUnitLevelId: createOrganizationUnit.OrganizationUnitLevelId,
            Description: createOrganizationUnit.Description,
            IsActive: createOrganizationUnit.IsActive,
            ParentId: createOrganizationUnit.ParentId
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrganizationUnit([FromRoute] Guid id, [FromBody] UpdateOrganizationUnit updateOrganizationUnit)
    {
        var command = new UpdateOrganizationUnitCommand(
            Id: id,
            Name: updateOrganizationUnit.Name,
            OrganizationUnitLevelId: updateOrganizationUnit.OrganizationUnitLevelId,
            Description: updateOrganizationUnit.Description,
            IsActive: updateOrganizationUnit.IsActive,
            ParentId: updateOrganizationUnit.ParentId
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetOrganizationUnits([FromQuery] QueryParam QueryRequest)
    {
        var query = new GetOrganizationUnitsQuery(QueryRequest);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrganizationUnitById([FromRoute] Guid id)
    {
        var query = new GetOrganizationUnitByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }
}
