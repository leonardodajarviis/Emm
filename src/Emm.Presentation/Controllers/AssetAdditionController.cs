using Emm.Application.Common;
using Emm.Application.Features.AppAssetAddition.Commands;
using Emm.Application.Features.AppAssetAddition.Queries;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/asset-additions")]
public class AssetAdditionController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssetAdditionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAssetAdditions([FromQuery] QueryParam queryParam)
    {
        var query = new GetAssetAdditionsQuery(queryParam);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssetAdditionById(Guid id)
    {
        var query = new GetAssetAdditionByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssetAddition([FromBody] CreateAssetAdditionCommand command)
    {
        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAssetAddition(Guid id, [FromBody] UpdateAssetAdditionCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in URL does not match ID in request body");
        }

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }
}
