using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Handler for the <see cref="CreateDepartmentCommand" />.
/// </summary>
public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, int>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="CreateDepartmentCommandHandler" />.
    /// </summary>
    public CreateDepartmentCommandHandler(
        IDepartmentRepository departmentRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService
    )
    {
        _departmentRepository = departmentRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles Command to create a new department.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Id of the new department.</returns>
    /// <exception cref="DepartmentNameAlreadyExistsException">Thrown if the name of the department already exists</exception>
    public async Task<int> Handle(
        CreateDepartmentCommand request,
        CancellationToken cancellationToken
    )
    {
        var department = new Department { DepartmentName = request.DepartmentName };
        if (
            !(
                await _authorizationService.CheckAccess(
                    department,
                    [AuthorizationConstants.Actions.CREATE]
                )
            )[AuthorizationConstants.Actions.CREATE]
        )
        {
            throw new UnauthorizedException();
        }
        if (await _departmentRepository.CheckIfDepartmentNameExistsAsync(request.DepartmentName))
        {
            throw new DepartmentNameAlreadyExistsException(request.DepartmentName);
        }
        await AddDepartmentLog(department);
        await _departmentRepository.AddDepartmentAsync(department);
        await _unitOfWork.CompleteAsync();

        return department.Id;
    }

    /// <summary>
    /// Adds Log entry for created Department
    /// </summary>
    /// <param name="department">newly created department</param>
    /// <returns></returns>
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
