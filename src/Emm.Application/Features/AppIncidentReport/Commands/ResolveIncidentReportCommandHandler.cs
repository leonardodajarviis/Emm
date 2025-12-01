using Emm.Application.Abstractions;
using Emm.Domain.Entities.Maintenance;

namespace Emm.Application.Features.AppIncidentReport.Commands;

public class ResolveIncidentReportCommandHandler : IRequestHandler<ResolveIncidentReportCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<IncidentReport, long> _repository;

    public ResolveIncidentReportCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<IncidentReport, long> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(ResolveIncidentReportCommand request, CancellationToken cancellationToken)
    {
        var incidentReport = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (incidentReport == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Incident Report not found");
        }

        incidentReport.Resolve(request.ResolutionNotes);
        
        _repository.Update(incidentReport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new { Id = incidentReport.Id });
    }
}