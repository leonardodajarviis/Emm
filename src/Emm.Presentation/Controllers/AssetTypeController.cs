using Emm.Application.Features.AppAssetType.Commands;
using Emm.Application.Features.AppAssetType.Dtos;
using Emm.Application.Features.AppAssetType.Queries;
using Emm.Application.Common;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/asset-types")]
public class AssetTypeController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssetTypeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssetType([FromBody] CreateAssetType createAssetType)
    {
        var command = new CreateAssetTypeCommand(
            Name: createAssetType.Name,
            AssetCategoryId: createAssetType.AssetCategoryId,
            ParameterIds: createAssetType.ParameterIds,
            Description: createAssetType.Description,
            IsActive: createAssetType.IsActive
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAssetType([FromRoute] long id, [FromBody] UpdateAssetType updateAssetType)
    {
        var command = new UpdateAssetTypeCommand(
            Id: id,
            Name: updateAssetType.Name,
            Description: updateAssetType.Description,
            AssetCategoryId: updateAssetType.AssetCategoryId,
            ParameterIds: updateAssetType.ParameterIds,
            IsActive: updateAssetType.IsActive
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

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAssetTypeById([FromRoute] long id)
    {
        var query = new GetAssetTypeByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }
}
