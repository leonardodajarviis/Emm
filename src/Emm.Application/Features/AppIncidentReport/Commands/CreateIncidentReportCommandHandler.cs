using Emm.Application.Abstractions;
using Emm.Domain.Entities.Maintenance;
using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppIncidentReport.Commands;

public class CreateIncidentReportCommandHandler : IRequestHandler<CreateIncidentReportCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<IncidentReport, long> _repository;
    private readonly IUserContextService _userContextService;
    private readonly IQueryContext _queryContext;

    public CreateIncidentReportCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<IncidentReport, long> repository,
        IUserContextService userContextService,
        IQueryContext queryContext)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(userContextService);
        ArgumentNullException.ThrowIfNull(queryContext);

        _unitOfWork = unitOfWork;
        _repository = repository;
        _userContextService = userContextService;
        _queryContext = queryContext;
    }

    public async Task<Result<object>> Handle(CreateIncidentReportCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var userId = _userContextService.GetCurrentUserId();
            if (userId == null)
            {
                return Result<object>.Failure(ErrorType.Unauthorized, "User not found");
            }

            // Validate Asset
            var assetExists = await _queryContext.Query<Asset>()
                .AnyAsync(x => x.Id == request.AssetId, cancellationToken);
            
            if (!assetExists)
            {
                return Result<object>.Failure(ErrorType.NotFound, "Asset not found");
            }

            var code = await _unitOfWork.GenerateNextCodeAsync("IR", "IncidentReports", 6, cancellationToken);

            var incidentReport = new IncidentReport(
                code: code,
                title: request.Title,
                description: request.Description,
                assetId: request.AssetId,
                reportedByUserId: userId.Value,
                priority: request.Priority
            );

            await _repository.AddAsync(incidentReport, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new
            {
                incidentReport.Id,
                incidentReport.Code
            });
        });
    }
}