using Emm.Application.Abstractions;
using Emm.Domain.Entities.Organization;

namespace Emm.Application.Features.AppLocation.Commands;

public class CreateLocationCommandHandler : IRequestHandler<CreateLocationCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Location, Guid> _repository;

    public CreateLocationCommandHandler(IUnitOfWork unitOfWork, IRepository<Location, Guid> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        // Generate unique code for location
        var code = await _unitOfWork.GenerateNextCodeAsync("VT", "Locations", 6, cancellationToken);
        
        var location = new Location(
            code: code,
            name: request.Name,
            organizationUnitId: request.OrganizationUnitId,
            description: request.Description,
            isActive: request.IsActive
        );

        await _repository.AddAsync(location);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = location.Id,
            Code = location.Code
        });
    }
}
