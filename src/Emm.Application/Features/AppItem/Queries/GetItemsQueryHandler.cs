using Emm.Application.Features.AppItem.Dtos;
using Emm.Domain.Entities;
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
        var query = _qq.Query<Item>().AsQueryable();

        var total = await query.CountAsync(cancellationToken);

        var items = await (
            from i in query
            join uom in _qq.Query<UnitOfMeasure>() on i.UnitOfMeasureId equals uom.Id into uomGroup
            from uom in uomGroup.DefaultIfEmpty()
            select new ItemSummaryDto
            {
                Id = i.Id,
                Code = i.Code,
                Name = i.Name,
                UnitOfMeasureId = i.UnitOfMeasureId,
                UnitOfMeasureName = uom != null ? uom.Name : string.Empty,

                UnitOfMeasureCategoryLines = (
                    from umcl in _qq.Query<UnitOfMeasureCategoryLine>()
                    join u in _qq.Query<UnitOfMeasure>() on umcl.UnitOfMeasureId equals u.Id
                    where umcl.UnitOfMeasureCategoryId == i.UnitOfMeasureCategoryId
                    select new UnitOfMeasureCategoryLineResponseDto
                    {
                        UnitOfMeasureId = umcl.UnitOfMeasureId,
                        UnitOfMeasureCode = u.Code,
                        UnitOfMeasureName = u.Name,
                    }).ToList()
            }
        ).ToListAsync(cancellationToken: cancellationToken);

        return Result<PagedResult>.Success(new PagedResult(
            page: request.QueryRequest.Page,
            pageSize: request.QueryRequest.PageSize,
            totalCount: total,
            results: items
        ));
    }
}
