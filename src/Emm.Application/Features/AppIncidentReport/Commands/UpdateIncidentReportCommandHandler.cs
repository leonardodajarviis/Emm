using Emm.Application.Abstractions;
using Emm.Application.Abstractions.Security;
using Emm.Domain.Entities.Maintenance;

namespace Emm.Application.Features.AppIncidentReport.Commands;

public class UpdateIncidentReportCommandHandler : IRequestHandler<UpdateIncidentReportCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<IncidentReport, Guid> _repository;
    private readonly ISecurityService _securityService;

    public UpdateIncidentReportCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<IncidentReport, Guid> repository,
        ISecurityService securityService)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _securityService = securityService;
    }

    public async Task<Result<object>> Handle(UpdateIncidentReportCommand request, CancellationToken cancellationToken)
    {
        var incidentReport = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (incidentReport == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Incident Report not found");
        }

        // ABAC Check
        if (!await _securityService.AuthorizeAsync(incidentReport, ResourceAction.Update, cancellationToken))
        {
            return Result<object>.Failure(ErrorType.Forbidden, "You do not have permission to update this incident report.");
        }

        incidentReport.UpdateInfo(request.Title, request.Description, request.Priority);
        
        _repository.Update(incidentReport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new { Id = incidentReport.Id });
    }
}
