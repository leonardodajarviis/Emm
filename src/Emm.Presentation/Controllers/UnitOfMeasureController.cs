using Emm.Application.Common;
using Emm.Application.Features.AppUnitOfMeasure.Commands;
using Emm.Application.Features.AppUnitOfMeasure.Dtos;
using Emm.Application.Features.AppUnitOfMeasure.Queries;
using Emm.Domain.Entities.Inventory;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/unit-of-measures")]
public class UnitOfMeasureController : ControllerBase
{
    private readonly IMediator _mediator;

    public UnitOfMeasureController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new unit of measure
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateUnitOfMeasure([FromBody] CreateUnitOfMeasure createUnitOfMeasure)
    {
        var command = new CreateUnitOfMeasureCommand(
            Name: createUnitOfMeasure.Name,
            Symbol: createUnitOfMeasure.Symbol,
            UnitType: createUnitOfMeasure.UnitType,
            Description: createUnitOfMeasure.Description,
            BaseUnitId: createUnitOfMeasure.BaseUnitId,
            ConversionFactor: createUnitOfMeasure.ConversionFactor
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    /// <summary>
    /// Update an existing unit of measure
    /// </summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateUnitOfMeasure([FromRoute] long id, [FromBody] UpdateUnitOfMeasure updateUnitOfMeasure)
    {
        var command = new UpdateUnitOfMeasureCommand(
            Id: id,
            Name: updateUnitOfMeasure.Name,
            Symbol: updateUnitOfMeasure.Symbol,
            UnitType: updateUnitOfMeasure.UnitType,
            Description: updateUnitOfMeasure.Description,
            BaseUnitId: updateUnitOfMeasure.BaseUnitId,
            ConversionFactor: updateUnitOfMeasure.ConversionFactor
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    /// <summary>
    /// Delete a unit of measure
    /// </summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteUnitOfMeasure([FromRoute] long id)
    {
        var command = new DeleteUnitOfMeasureCommand(Id: id);
        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    /// <summary>
    /// Activate a unit of measure
    /// </summary>
    [HttpPatch("{id:long}/activate")]
    public async Task<IActionResult> ActivateUnitOfMeasure([FromRoute] long id)
    {
        var command = new ActivateUnitOfMeasureCommand(Id: id);
        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    /// <summary>
    /// Deactivate a unit of measure
    /// </summary>
    [HttpPatch("{id:long}/deactivate")]
    public async Task<IActionResult> DeactivateUnitOfMeasure([FromRoute] long id)
    {
        var command = new DeactivateUnitOfMeasureCommand(Id: id);
        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    /// <summary>
    /// Get all units of measure with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetUnitOfMeasures([FromQuery] QueryParam QueryRequest)
    {
        var query = new GetUnitOfMeasuresQuery(QueryRequest);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    /// <summary>
    /// Get a unit of measure by ID
    /// </summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetUnitOfMeasureById([FromRoute] long id)
    {
        var query = new GetUnitOfMeasureByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }
}
