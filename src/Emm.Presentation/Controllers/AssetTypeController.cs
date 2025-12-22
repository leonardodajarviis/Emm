using Emm.Application.Features.AppAssetType.Commands;
using Emm.Application.Features.AppAssetType.Dtos;
using Emm.Application.Features.AppAssetType.Queries;
using Emm.Application.Common;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Emm.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/asset-types")]
public class AssetTypeController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssetTypeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssetType([FromBody] CreateAssetTypeCommand command)
    {
        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAssetType([FromRoute] Guid id, [FromBody] UpdateAssetTypeBody updateAssetType)
    {
        var command = new UpdateAssetTypeCommand(
            id,
            updateAssetType
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAssetTypes([FromQuery] QueryParam QueryRequest)
    {
        var query = new GetAssetTypesQuery(QueryRequest);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssetTypeById([FromRoute] Guid id)
    {
        var query = new GetAssetTypeByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }
}
