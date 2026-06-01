using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Handler for the <see cref="UpdateDepartmentCommand" />.
/// </summary>
public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Department>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new instance of <see cref="UpdateDepartmentCommandHandler" />.
    /// </summary>
    public UpdateDepartmentCommandHandler(
        IDepartmentRepository departmentRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _departmentRepository = departmentRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles a command for updating a department.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Updated Department.</returns>
    /// <exception cref="DepartmentNameAlreadyExistsException">When the new department name already exists.</exception>
    public async Task<Department> Handle(
        UpdateDepartmentCommand request,
        CancellationToken cancellationToken
    )
    {
        var department = await _departmentRepository.GetDepartmentAsync(request.Id);
        var logChanges = new List<LogChange> { };
        if (request.DepartmentName != null && request.DepartmentName != department.DepartmentName)
        {
            if (
                !string.Equals(
                    request.DepartmentName,
                    department.DepartmentName,
                    System.StringComparison.OrdinalIgnoreCase
                )
                && await _departmentRepository.CheckIfDepartmentNameExistsAsync(
                    request.DepartmentName
                )
            )
            {
                throw new DepartmentNameAlreadyExistsException(request.DepartmentName);
            }

            logChanges.Add(
                new LogChange
                {
                    Property = nameof(Department.DepartmentName),
                    OldValue = department.DepartmentName,
                    NewValue = request.DepartmentName,
                }
            );
            department.DepartmentName = request.DepartmentName;
        }
        if (logChanges.Count > 0)
        {
            var updatedDepartment = await _departmentRepository.UpdateDepartmentAsync(department);
            await _logRepository.AddDepartmentLogForCurrentActor(
                department: department,
                action: Action.UPDATED_DEPARTMENT,
                logChanges
            );
            await _unitOfWork.CompleteAsync();
            return updatedDepartment;
        }

        return department;
    }
}
