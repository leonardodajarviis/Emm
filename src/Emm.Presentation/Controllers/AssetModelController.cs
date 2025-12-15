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

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAssetModel([FromRoute] long id, [FromBody] UpdateAssetModel updateAssetModel)
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

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAssetModelById([FromRoute] long id)
    {
        var query = new GetAssetModelByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    [HttpPost("{id:long}/maintenance-plans")]
    public async Task<IActionResult> AddMaintenancePlan(
        [FromRoute] long id,
        [FromBody] AddMaintenancePlan addMaintenancePlan)
    {
        var command = new AddMaintenancePlanCommand(
            AssetModelId: id,
            Name: addMaintenancePlan.Name,
            Description: addMaintenancePlan.Description,
            IsActive: addMaintenancePlan.IsActive,
            PlanType: addMaintenancePlan.PlanType,
            RRule: addMaintenancePlan.RRule,
            ParameterId: addMaintenancePlan.ParameterId,
            TriggerValue: addMaintenancePlan.TriggerValue,
            MinValue: addMaintenancePlan.MinValue,
            MaxValue: addMaintenancePlan.MaxValue,
            TriggerCondition: addMaintenancePlan.TriggerCondition,
            JobSteps: addMaintenancePlan.JobSteps?.Select(js => new AddMaintenancePlanJobStepCommand(
                Name: js.Name,
                OrganizationUnitId: js.OrganizationUnitId,
                Note: js.Note,
                Order: js.Order
            )).ToList(),
            RequiredItems: addMaintenancePlan.RequiredItems?.Select(ri => new AddMaintenancePlanRequiredItemCommand(
                ItemId: ri.ItemId,
                Quantity: ri.Quantity,
                IsRequired: ri.IsRequired,
                Note: ri.Note
            )).ToList()
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }


    [HttpPut("{id:long}/maintenance-plans/{maintenancePlanId:long}")]
    public async Task<IActionResult> UpdateMaintenancePlan(
        [FromRoute] long id,
        [FromRoute] long maintenancePlanId,
        [FromBody] UpdateMaintenancePlanDto updateMaintenancePlan)
    {
        var command = new UpdateMaintenancePlanCommand(
            AssetModelId: id,
            MaintenancePlanId: maintenancePlanId,
            Name: updateMaintenancePlan.Name,
            Description: updateMaintenancePlan.Description,
            IsActive: updateMaintenancePlan.IsActive,
            PlanType: updateMaintenancePlan.PlanType,
            RRule: updateMaintenancePlan.RRule,
            TriggerValue: updateMaintenancePlan.TriggerValue,
            MinValue: updateMaintenancePlan.MinValue,
            MaxValue: updateMaintenancePlan.MaxValue,
            TriggerCondition: updateMaintenancePlan.TriggerCondition,
            JobSteps: updateMaintenancePlan.JobSteps?.Select(js => new UpdateJobStepCommand(
                Id: js.Id,
                Name: js.Name,
                OrganizationUnitId: js.OrganizationUnitId,
                Note: js.Note,
                Order: js.Order
            )).ToList(),
            RequiredItems: updateMaintenancePlan.RequiredItems?.Select(ri => new UpdateRequiredItemCommand(
                Id: ri.Id,
                ItemId: ri.ItemId,
                Quantity: ri.Quantity,
                IsRequired: ri.IsRequired,
                Note: ri.Note
            )).ToList()
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpDelete("{id:long}/maintenance-plans/{maintenancePlanId:long}")]
    public async Task<IActionResult> RemoveMaintenancePlan(
        [FromRoute] long id,
        [FromRoute] long maintenancePlanId)
    {
        var command = new RemoveMaintenancePlanCommand(
            AssetModelId: id,
            MaintenancePlanId: maintenancePlanId
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPost("{id:long}/images")]
    public async Task<IActionResult> AddImagesToAssetModel(
        [FromRoute] long id,
        [FromBody] List<Guid> fileIds)
    {
        var command = new AddImagesToAssetModelCommand(
            AssetModelId: id,
            FileIds: fileIds
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpDelete("{id:long}/images")]
    public async Task<IActionResult> RemoveImagesFromAssetModel(
        [FromRoute] long id,
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
    [HttpPatch("{id:long}/thumbnail")]
    public async Task<IActionResult> ChangeThumbnail(
        [FromRoute] long id,
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
