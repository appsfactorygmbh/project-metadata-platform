using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Departments;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, int>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDepartmentCommandHandler(
        IDepartmentRepository departmentRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _departmentRepository = departmentRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(
        CreateDepartmentCommand request,
        CancellationToken cancellationToken
    )
    {
        if (await _departmentRepository.CheckIfDepartmentNameExistsAsync(request.DepartmentName))
        {
            throw new DepartmentNameAlreadyExistsException(request.DepartmentName);
        }

        var department = new Department { DepartmentName = request.DepartmentName };
        await AddDepartmentLog(department);
        await _departmentRepository.AddDepartmentAsync(department);
        await _unitOfWork.CompleteAsync();

        return department.Id;
    }

    private async Task AddDepartmentLog(Department department)
    {
        var logChanges = new List<LogChange>
        {
            new LogChange
            {
                Property = nameof(Department.DepartmentName),
                OldValue = "",
                NewValue = department.DepartmentName,
            },
        };

        await _logRepository.AddDepartmentLogForCurrentActor(
            department,
            Action.ADDED_DEPARTMENT,
            logChanges
        );
    }
}
