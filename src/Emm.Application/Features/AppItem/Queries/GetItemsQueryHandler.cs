using Emm.Application.Features.AppItem.Dtos;
using Emm.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppItem.Queries;

public class GetItemsQueryHandler : IRequestHandler<GetItemsQuery, Result<PagedResult>>
{
    private readonly IQueryContext _qq;
    public GetItemsQueryHandler(IQueryContext qq)
    {
        _qq = qq;
    }

    public async Task<Result<PagedResult>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        var query = _qq.Query<Item>()
            .AsQueryable();

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Select(i => new ItemSummaryDto
        {
            Id = i.Id,
            Code = i.Code,
            Name = i.Name,
            UnitOfMeasureId = i.UnitOfMeasureId,
            UnitOfMeasureName = _qq.Query<UnitOfMeasure>()
                    .Where(u => u.Id == i.UnitOfMeasureId)
                    .Select(u => u.Name)
                    .FirstOrDefault() ?? string.Empty
        }).ToListAsync(cancellationToken: cancellationToken);

        return Result<PagedResult>.Success(new PagedResult(
            page: request.QueryRequest.Page,
            pageSize: request.QueryRequest.PageSize,
            totalCount: total,
            results: items
        ));
    }
}
