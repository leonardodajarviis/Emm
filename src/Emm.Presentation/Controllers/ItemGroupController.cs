using Emm.Application.Common;
using Emm.Application.Features.AppItemGroup.Queries;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/item-groups")]
public class ItemGroupController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemGroupController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all item groups with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetItemGroups([FromQuery] QueryParam queryRequest)
    {
        var query = new GetItemGroupsQuery(queryRequest);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }
}
