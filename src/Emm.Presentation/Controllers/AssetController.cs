using Emm.Application.Features.AppAsset.Commands;
using Emm.Application.Features.AppAsset.Queries;
using Emm.Application.Common;
using Emm.Application.Features.AppAsset.Dtos;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/assets")]
public class AssetController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssetController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsset([FromBody] CreateAssetCommand command)
    {
        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsset([FromRoute] Guid id, [FromBody] UpdateAsset updateAsset)
    {
        var command = new UpdateAssetCommand(
            Id: id,
            DisplayName: updateAsset.DisplayName,
            Description: updateAsset.Description
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAssets([FromQuery] QueryParam QueryRequest)
    {
        var query = new GetAssetsQuery(QueryRequest);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAssetById([FromRoute] Guid id)
    {
        var query = new GetAssetByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }
}
