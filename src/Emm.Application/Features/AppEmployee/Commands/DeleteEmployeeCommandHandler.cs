using Emm.Domain.Entities.Organization;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppEmployee.Commands;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Employee, long> _repository;

    public DeleteEmployeeCommandHandler(IUnitOfWork unitOfWork, IRepository<Employee, long> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (employee == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Employee not found.");
        }

        _repository.Remove(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = request.Id
        });
    }
}
