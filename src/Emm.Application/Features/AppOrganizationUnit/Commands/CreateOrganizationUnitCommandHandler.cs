using Emm.Application.Abstractions;
using Emm.Domain.Entities.Organization;

namespace Emm.Application.Features.AppOrganizationUnit.Commands;

public class CreateOrganizationUnitCommandHandler : IRequestHandler<CreateOrganizationUnitCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<OrganizationUnit, long> _repository;

    public CreateOrganizationUnitCommandHandler(IUnitOfWork unitOfWork, IRepository<OrganizationUnit, long> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(CreateOrganizationUnitCommand request, CancellationToken cancellationToken)
    {
        // Generate unique code for organization unit
        var code = await _unitOfWork.GenerateNextCodeAsync("DV", "OrganizationUnits", 6, cancellationToken);
        
        var organizationUnit = new OrganizationUnit(
            code: code,
            name: request.Name,
            organizationUnitLevelId: request.OrganizationUnitLevelId,
            description: request.Description,
            isActive: request.IsActive,
            parentId: request.ParentId
        );

        await _repository.AddAsync(organizationUnit);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = organizationUnit.Id,
            Code = organizationUnit.Code
        });
    }
}
