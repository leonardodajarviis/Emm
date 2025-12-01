using Emm.Application.Abstractions;
using Emm.Application.Features.AppIncidentReport.Dtos;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Maintenance;
using Gridify;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppIncidentReport.Queries;

public class GetIncidentReportsQueryHandler : IRequestHandler<GetIncidentReportsQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;

    public GetIncidentReportsQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<PagedResult>> Handle(GetIncidentReportsQuery request, CancellationToken cancellationToken)
    {
        var query = _queryContext.Query<IncidentReport>().ApplyFiltering(request.QueryRequest);

        if (!string.IsNullOrEmpty(request.QueryRequest.GetSearch()))
        {
            var search = request.QueryRequest.GetSearch()!;
            query = query.Where(x => x.Title.Contains(search) || x.Code.Contains(search));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Select(ir => new IncidentReportResponse
            {
                Id = ir.Id,
                Code = ir.Code,
                Title = ir.Title,
                Description = ir.Description,
                AssetId = ir.AssetId,
                AssetName = _queryContext.Query<Asset>()
                    .Where(a => a.Id == ir.AssetId)
                    .Select(a => a.DisplayName)
                    .FirstOrDefault(),
                CreatedById = ir.CreatedById,
                CreatedBy = _queryContext.Query<User>()
                    .Where(u => u.Id == ir.CreatedById)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),
                ReportedAt = ir.ReportedAt,
                Priority = ir.Priority,
                Status = ir.Status,
                ResolvedAt = ir.ResolvedAt,
                ResolutionNotes = ir.ResolutionNotes,
                CreatedAt = ir.CreatedAt,
                UpdatedAt = ir.UpdatedAt
            })
            .OrderByDescending(x => x.CreatedAt)
            .ApplyOrderingAndPaging(request.QueryRequest)
            .ToListAsync(cancellationToken);

        return Result<PagedResult>.Success(request.QueryRequest.AsPagedResult(total, items));
    }
}
