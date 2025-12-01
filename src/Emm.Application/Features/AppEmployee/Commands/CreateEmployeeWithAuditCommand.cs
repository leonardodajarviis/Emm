using Emm.Application.Abstractions;
using Emm.Application.Common;
using Emm.Domain.Entities.Organization;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppEmployee.Commands;

public class CreateEmployeeWithAuditCommand : IRequest<Result<object>>
{
    public string DisplayName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public long OrganizationUnitId { get; set; }
}

public class CreateEmployeeWithAuditCommandHandler : IRequestHandler<CreateEmployeeWithAuditCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Employee, long> _repository;
    private readonly IUserContextService _userContextService;

    public CreateEmployeeWithAuditCommandHandler(
        IUnitOfWork unitOfWork, 
        IRepository<Employee, long> repository,
        IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _userContextService = userContextService;
    }

    public async Task<Result<object>> Handle(CreateEmployeeWithAuditCommand request, CancellationToken cancellationToken)
    {
        // Get current user information for audit
        var currentUserId = _userContextService.GetCurrentUserId();
        var currentUsername = _userContextService.GetCurrentUsername();
        
        if (currentUserId == null)
        {
            return Result<object>.Failure(ErrorType.Unauthorized, "User not authenticated");
        }

        var code = await _unitOfWork.GenerateNextCodeAsync("EMP", "Employees", 6, cancellationToken);

        var employee = new Employee(
            code: code,
            displayName: request.DisplayName,
            firstName: request.FirstName,
            lastName: request.LastName,
            organizationId: request.OrganizationUnitId
        );

        // Add audit information using domain events or separate audit entity
        // This is just an example of how to use UserContextService
        
        await _repository.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new 
        { 
            Id = employee.Id, 
            Code = employee.Code,
            CreatedBy = currentUsername,
            CreatedByUserId = currentUserId
        });
    }
}
