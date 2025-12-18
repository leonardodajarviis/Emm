using Emm.Application.Features.AppAssetCategory.Commands;
using Emm.Application.Features.AppAssetCategory.Queries;
using Emm.Application.Common;
using Emm.Application.Features.AppAssetCategory.Dtos;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/asset-categories")]
public class AssetCategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssetCategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssetCategory([FromBody] CreateAssetCategoryCommand command)
    {
        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAssetCategory([FromRoute] Guid id, [FromBody] UpdateAssetCategory updateAssetCategory)
    {
        var command = new UpdateAssetCategoryCommand(
            Id: id,
            Name: updateAssetCategory.Name,
            Description: updateAssetCategory.Description,
            IsActive: updateAssetCategory.IsActive
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAssetCategories([FromQuery] QueryParam QueryRequest)
    {
        var query = new GetAssetCategoriesQuery(QueryRequest);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssetCategoryById([FromRoute] Guid id)
    {
        var query = new GetAssetCategoryByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }
}

