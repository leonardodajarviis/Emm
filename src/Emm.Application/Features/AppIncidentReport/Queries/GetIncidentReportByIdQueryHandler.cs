using Emm.Application.Abstractions;
using Emm.Application.Features.AppIncidentReport.Dtos;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Maintenance;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppIncidentReport.Queries;

public class GetIncidentReportByIdQueryHandler : IRequestHandler<GetIncidentReportByIdQuery, Result<IncidentReportResponse>>
{
    private readonly IQueryContext _qq;

    public GetIncidentReportByIdQueryHandler(IQueryContext queryContext)
    {
        _qq = queryContext;
    }

    public async Task<Result<IncidentReportResponse>> Handle(GetIncidentReportByIdQuery request, CancellationToken cancellationToken)
    {
        var incidentReport = await _qq.Query<IncidentReport>()
            .Where(x => x.Id == request.Id)
            .Select(x => new IncidentReportResponse
            {
                Id = x.Id,
                Code = x.Code,
                Title = x.Title,
                Description = x.Description,
                AssetId = x.AssetId,
                AssetName = _qq.Query<Asset>()
                    .Where(a => a.Id == x.AssetId)
                    .Select(a => a.DisplayName)
                    .FirstOrDefault(),
                CreatedById = x.Audit.CreatedByUserId,
                CreatedBy = _qq.Query<User>()
                    .Where(u => u.Id == x.Audit.CreatedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),
                ReportedAt = x.ReportedAt,
                Priority = x.Priority,
                Status = x.Status,
                ResolvedAt = x.ResolvedAt,
                ResolutionNotes = x.ResolutionNotes,
                CreatedAt = x.Audit.CreatedAt,
                ModifiedAt = x.Audit.ModifiedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (incidentReport == null)
        {
            return Result<IncidentReportResponse>.Failure(ErrorType.NotFound, "Incident Report not found");
        }

        return Result<IncidentReportResponse>.Success(incidentReport);
    }
}
