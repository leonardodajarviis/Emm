using Emm.Application.Features.AppOperationShift.Dtos;
using Emm.Domain.Entities;
using Emm.Domain.Entities.Operations;
using Gridify;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Queries;

public class GetOperationShiftsQueryHandler : IRequestHandler<GetOperationShiftsQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;

    public GetOperationShiftsQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<PagedResult>> Handle(GetOperationShiftsQuery request, CancellationToken cancellationToken)
    {
        var query = _queryContext.Query<OperationShift>();

        // Apply search filter - Note: QueryRequest doesn't have Search property, so skipping for now
        // if (!string.IsNullOrEmpty(request.QueryRequest.Search))
        // {
        //     query = query.Where(x => x.Code.Contains(request.QueryRequest.Search) ||
        //                            x.Name.Contains(request.QueryRequest.Search) ||
        //                            x.PrimaryOperatorName.Contains(request.QueryRequest.Search));
        // }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var operationShifts = await query
            .OrderByDescending(os => os.Id)
            .ApplyOrderingAndPaging(request.QueryRequest)
            .Select(os => new OperationShiftSummaryResponse
            {
                Id = os.Id,
                Code = os.Code,
                Name = os.Name,
                Description = os.Description,
                IsCheckpointLogEnabled = os.IsCheckpointLogEnabled,
                OrganizationUnitId = os.OrganizationUnitId,
                PrimaryUserId = os.PrimaryUserId,
                PrimaryUserDisplayName = _queryContext.Query<User>()
                    .Where(u => u.Id == os.PrimaryUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),
                ScheduledStartTime = os.ScheduledStartTime,
                ScheduledEndTime = os.ScheduledEndTime,
                ActualStartTime = os.ActualStartTime,
                ActualEndTime = os.ActualEndTime,
                Status = os.Status.Value,
                Notes = os.Notes,
                CreatedAt = os.Audit.CreatedAt,
                ModifiedAt = os.Audit.ModifiedAt
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult(
            page: request.QueryRequest.Page,
            pageSize: request.QueryRequest.PageSize,
            totalCount: totalCount,
            results: operationShifts
        );

        return Result<PagedResult>.Success(pagedResult);
    }
}
