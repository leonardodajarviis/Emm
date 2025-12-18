using Emm.Application.Abstractions;
using Emm.Domain.Entities.Organization;

namespace Emm.Application.Features.AppOrganizationUnitLevel.Commands;

public class UpdateOrganizationUnitLevelCommandHandler : IRequestHandler<UpdateOrganizationUnitLevelCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<OrganizationUnitLevel, Guid> _repository;

    public UpdateOrganizationUnitLevelCommandHandler(IUnitOfWork unitOfWork, IRepository<OrganizationUnitLevel, Guid> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(UpdateOrganizationUnitLevelCommand request, CancellationToken cancellationToken)
    {
        var organizationUnitLevel = await _repository.GetByIdAsync(request.Id);
        if (organizationUnitLevel == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Organization unit level not found");
        }

        organizationUnitLevel.Update(request.Name, request.Description, request.Level);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new { Id = organizationUnitLevel.Id });
    }
}
