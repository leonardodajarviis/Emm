using Emm.Domain.Entities.Organization;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppEmployee.Commands;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Employee, long> _repository;

    public CreateEmployeeCommandHandler(IUnitOfWork unitOfWork, IRepository<Employee, long> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var code = await _unitOfWork.GenerateNextCodeAsync("NV", "Employees", 6, cancellationToken);

        var employee = new Employee(
            code: code,
            displayName: request.DisplayName,
            firstName: request.FirstName,
            lastName: request.LastName,
            organizationId: request.OrganizationUnitId
        );

        await _repository.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = employee.Id,
            Code = employee.Code
        });
    }
}
