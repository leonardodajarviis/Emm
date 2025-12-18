using Emm.Domain.Entities.Organization;

namespace Emm.Application.Features.AppOrganizationUnitLevel.Commands;

public class CreateOrganizationUnitLevelCommandHandler : IRequestHandler<CreateOrganizationUnitLevelCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<OrganizationUnitLevel, Guid> _repository;

    public CreateOrganizationUnitLevelCommandHandler(IUnitOfWork unitOfWork, IRepository<OrganizationUnitLevel, Guid> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(CreateOrganizationUnitLevelCommand request, CancellationToken cancellationToken)
    {
        var organizationUnitLevel = new OrganizationUnitLevel(
            name: request.Name,
            description: request.Description,
            level: request.Level
        );

        await _repository.AddAsync(organizationUnitLevel);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = organizationUnitLevel.Id
        });
    }
}
