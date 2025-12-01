using Emm.Application.Abstractions;
using Emm.Domain.Entities.Maintenance;

namespace Emm.Application.Features.AppIncidentReport.Commands;

public class IncidentReportStateCommandsHandler : 
    IRequestHandler<AssignIncidentReportCommand, Result<object>>,
    IRequestHandler<StartIncidentReportProgressCommand, Result<object>>,
    IRequestHandler<CloseIncidentReportCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<IncidentReport, long> _repository;

    public IncidentReportStateCommandsHandler(
        IUnitOfWork unitOfWork,
        IRepository<IncidentReport, long> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(AssignIncidentReportCommand request, CancellationToken cancellationToken)
    {
        var incidentReport = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (incidentReport == null) return Result<object>.Failure(ErrorType.NotFound, "Incident Report not found");

        incidentReport.Assign();
        _repository.Update(incidentReport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<object>.Success(new { Id = incidentReport.Id });
    }

    public async Task<Result<object>> Handle(StartIncidentReportProgressCommand request, CancellationToken cancellationToken)
    {
        var incidentReport = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (incidentReport == null) return Result<object>.Failure(ErrorType.NotFound, "Incident Report not found");

        incidentReport.StartProgress();
        _repository.Update(incidentReport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<object>.Success(new { Id = incidentReport.Id });
    }

    public async Task<Result<object>> Handle(CloseIncidentReportCommand request, CancellationToken cancellationToken)
    {
        var incidentReport = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (incidentReport == null) return Result<object>.Failure(ErrorType.NotFound, "Incident Report not found");

        incidentReport.Close();
        _repository.Update(incidentReport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<object>.Success(new { Id = incidentReport.Id });
    }
}