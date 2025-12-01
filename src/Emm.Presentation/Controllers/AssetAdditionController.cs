using Emm.Application.Features.AppAssetAddition.Commands;
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

    [HttpPost]
    public async Task<IActionResult> CreateAssetAddition([FromBody] CreateAssetAdditionCommand command)
    {
        var result = await _mediator.Send(command);
        
        return result.ToActionResult();
    }
}