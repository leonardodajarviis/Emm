using Emm.Application.Common;
using Emm.Application.Features.AppAssetModel.Commands;
using Emm.Application.Features.AppAssetModel.Dtos;
using Emm.Application.Features.AppAssetModel.Queries;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/asset-models")]
public class AssetModelController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssetModelController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssetModel([FromBody] CreateAssetModelCommand command)
    {
        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAssetModel([FromRoute] Guid id, [FromBody] UpdateAssetModel updateAssetModel)
    {
        var command = new UpdateAssetModelCommand(
            Id: id,
            Name: updateAssetModel.Name,
            Description: updateAssetModel.Description,
            Notes: updateAssetModel.Notes,
            ParentId: updateAssetModel.ParentId,
            AssetCategoryId: updateAssetModel.AssetCategoryId,
            AssetTypeId: updateAssetModel.AssetTypeId,
            IsActive: updateAssetModel.IsActive
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAssetModels([FromQuery] QueryParam queryRequest)
    {
        var query = new GetAssetModelsQuery(queryRequest);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssetModelById([FromRoute] Guid id)
    {
        var query = new GetAssetModelByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    [HttpPost("{id}/maintenance-plans")]
    public async Task<IActionResult> AddMaintenancePlan(
        [FromRoute] Guid id,
        [FromBody] AddMaintenancePlanCommandBody addMaintenancePlan)
    {
        var command = new AddMaintenancePlanCommand(
            AssetModelId: id,
            Body: addMaintenancePlan
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }


    [HttpPut("{id}/maintenance-plans/{maintenancePlanId}")]
    public async Task<IActionResult> UpdateMaintenancePlan(
        [FromRoute] Guid id,
        [FromRoute] Guid maintenancePlanId,
        [FromBody] UpdateMaintenancePlanBody updateMaintenancePlan)
    {
        var command = new UpdateMaintenancePlanCommand(
            AssetModelId: id,
            MaintenancePlanId: maintenancePlanId,
            Body: updateMaintenancePlan
        ) ;

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpDelete("{id}/maintenance-plans/{maintenancePlanId}")]
    public async Task<IActionResult> RemoveMaintenancePlan(
        [FromRoute] Guid id,
        [FromRoute] Guid maintenancePlanId)
    {
        var command = new RemoveMaintenancePlanCommand(
            AssetModelId: id,
            MaintenancePlanId: maintenancePlanId
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImagesToAssetModel(
        [FromRoute] Guid id,
        [FromBody] List<Guid> fileIds)
    {
        var command = new AddImagesToAssetModelCommand(
            AssetModelId: id,
            FileIds: fileIds
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpDelete("{id}/images")]
    public async Task<IActionResult> RemoveImagesFromAssetModel(
        [FromRoute] Guid id,
        [FromBody] List<Guid> fileIds)
    {
        var command = new RemoveImagesFromAssetModelCommand(
            AssetModelId: id,
            FileIds: fileIds
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    /// <summary>
    /// Change thumbnail for an asset model. If the fileId exists in images, it will be removed from images.
    /// </summary>
    [HttpPatch("{id}/thumbnail")]
    public async Task<IActionResult> ChangeThumbnail(
        [FromRoute] Guid id,
        [FromBody] ChangeThumbnail changeThumbnail)
    {
        var command = new ChangeThumbnailCommand(
            AssetModelId: id,
            FileId: changeThumbnail.FileId
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }
}
