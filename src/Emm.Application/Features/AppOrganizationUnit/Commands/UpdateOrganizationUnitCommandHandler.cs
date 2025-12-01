using Emm.Domain.Entities.Organization;

namespace Emm.Application.Features.AppOrganizationUnit.Commands;

public class UpdateOrganizationUnitCommandHandler : IRequestHandler<UpdateOrganizationUnitCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<OrganizationUnit, long> _repository;

    public UpdateOrganizationUnitCommandHandler(IUnitOfWork unitOfWork, IRepository<OrganizationUnit, long> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(UpdateOrganizationUnitCommand request, CancellationToken cancellationToken)
    {
        var organizationUnit = await _repository.GetByIdAsync(request.Id);
        if (organizationUnit == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Organization unit not found");
        }

        organizationUnit.Update(request.Name, request.OrganizationUnitLevelId, request.Description, request.IsActive, request.ParentId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new { Id = organizationUnit.Id });
    }
}
