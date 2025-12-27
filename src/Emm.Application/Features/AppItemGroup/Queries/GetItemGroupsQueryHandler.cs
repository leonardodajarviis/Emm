using Emm.Domain.Entities.Inventory;
using Gridify;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppItemGroup.Queries;


public class GetItemGroupsQueryHandler : IRequestHandler<GetItemGroupsQuery, Result<PagedResult>>
{
    private readonly IQueryContext _itemGroupRepository;

    public GetItemGroupsQueryHandler(
        IQueryContext itemGroupRepository)
    {
        _itemGroupRepository = itemGroupRepository;
    }

    public async Task<Result<PagedResult>> Handle(GetItemGroupsQuery request, CancellationToken cancellationToken)
    {
        var pr = request.QueryRequest;
        var query = _itemGroupRepository.Query<ItemGroup>().AsQueryable().ApplyFiltering(pr);

        var total = await query.CountAsync(cancellationToken);
        var itemGroups = await query
            .OrderByDescending(x => x.Id)
            .ApplyOrderingAndPaging(pr)
            .Select(ig => new ItemGroupSummaryDto
            {
                Id = ig.Id,
                Code = ig.Code.Value,
                Name = ig.Name,
                Description = ig.Description,
                IsActive = ig.IsActive
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult>.Success(pr.AsPagedResult(total, itemGroups));
    }
}
