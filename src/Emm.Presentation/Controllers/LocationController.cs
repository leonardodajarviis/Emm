using Emm.Application.Features.AppLocation.Commands;
using Emm.Application.Features.AppLocation.Dtos;
using Emm.Application.Features.AppLocation.Queries;
using Emm.Application.Common;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] CreateLocation createLocation)
    {
        var command = new CreateLocationCommand(
            Name: createLocation.Name,
            OrganizationUnitId: createLocation.OrganizationUnitId,
            Description: createLocation.Description,
            IsActive: createLocation.IsActive
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLocation([FromRoute] Guid id, [FromBody] UpdateLocation updateLocation)
    {
        var command = new UpdateLocationCommand(
            Id: id,
            Name: updateLocation.Name,
            Description: updateLocation.Description,
            IsActive: updateLocation.IsActive
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetLocations([FromQuery] QueryParam QueryRequest)
    {
        var query = new GetLocationsQuery(QueryRequest);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLocationById([FromRoute] Guid id)
    {
        var query = new GetLocationByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }
}
