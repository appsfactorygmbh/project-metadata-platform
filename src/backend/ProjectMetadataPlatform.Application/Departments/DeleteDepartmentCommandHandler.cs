using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Departments;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDepartmentCommandHandler(
        IDepartmentRepository departmentRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _departmentRepository = departmentRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetDepartmentAsync(request.Id);
        _ = await _departmentRepository.DeleteDepartmentAsync(department);
        await _logRepository.AddDepartmentLogForCurrentActor(
            department,
            Action.REMOVED_DEPARTMENT,
            []
        );
        await _unitOfWork.CompleteAsync();
    }
}
