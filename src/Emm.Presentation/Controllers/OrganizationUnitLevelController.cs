using Emm.Application.Features.AppOrganizationUnitLevel.Commands;
using Emm.Application.Features.AppOrganizationUnitLevel.Dtos;
using Emm.Application.Features.AppOrganizationUnitLevel.Queries;
using Emm.Application.Common;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/organization-unit-levels")]
public class OrganizationUnitLevelController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganizationUnitLevelController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganizationUnitLevel([FromBody] CreateOrganizationUnitLevel createOrganizationUnitLevel)
    {
        var command = new CreateOrganizationUnitLevelCommand(
            Name: createOrganizationUnitLevel.Name,
            Level: createOrganizationUnitLevel.Level,
            Description: createOrganizationUnitLevel.Description
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrganizationUnitLevel([FromRoute] Guid id, [FromBody] UpdateOrganizationUnitLevel updateOrganizationUnitLevel)
    {
        var command = new UpdateOrganizationUnitLevelCommand(
            Id: id,
            Name: updateOrganizationUnitLevel.Name,
            Description: updateOrganizationUnitLevel.Description,
            Level: updateOrganizationUnitLevel.Level
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetOrganizationUnitLevels([FromQuery] QueryParam QueryRequest)
    {
        var query = new GetOrganizationUnitLevelsQuery(QueryRequest);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrganizationUnitLevelById([FromRoute] Guid id)
    {
        var query = new GetOrganizationUnitLevelByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }
}
