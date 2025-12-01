using Emm.Application.Features.AppParameterCatalog.Commands;
using Emm.Application.Features.AppParameterCatalog.Dtos;
using Emm.Application.Features.AppParameterCatalog.Queries;
using Emm.Application.Common;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/parameter-catalogs")]
public class ParameterCatalogController : ControllerBase
{
    private readonly IMediator _mediator;

    public ParameterCatalogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateParameterCatalog([FromBody] CreateParameterCatalog createParameterCatalog)
    {
        var command = new CreateParameterCatalogCommand(
            Name: createParameterCatalog.Name,
            Description: createParameterCatalog.Description
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateParameterCatalog([FromRoute] long id, [FromBody] UpdateParameterCatalog updateParameterCatalog)
    {
        var command = new UpdateParameterCatalogCommand(
            Id: id,
            Name: updateParameterCatalog.Name,
            Description: updateParameterCatalog.Description
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteParameterCatalog([FromRoute] long id)
    {
        var command = new DeleteParameterCatalogCommand(Id: id);
        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetParameterCatalogs([FromQuery] QueryParam QueryRequest)
    {
        var query = new GetParameterCatalogsQuery(QueryRequest);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetParameterCatalogById([FromRoute] long id)
    {
        var query = new GetParameterCatalogByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }
}
