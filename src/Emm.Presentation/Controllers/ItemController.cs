using Emm.Application.Common;
using Emm.Application.Features.AppItem.Queries;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/items")]
public class AppItemController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all items with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetItems([FromQuery] QueryParam queryRequest)
    {
        var query = new GetItemsQuery(queryRequest);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }
}
