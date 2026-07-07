using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Handler for the <see cref="DeleteDepartmentCommand" />.
/// </summary>
public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="DeleteDepartmentCommandHandler" />.
    /// </summary>
    public DeleteDepartmentCommandHandler(
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
    /// Handler for Command to delete a Department.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetDepartmentAsync(request.Id);
        if (
            !(
                await _authorizationService.CheckAccess(
                    department,
                    [AuthorizationConstants.Actions.DELETE]
                )
            )[AuthorizationConstants.Actions.DELETE]
        )
        {
            throw new UnauthorizedException();
        }
        _ = await _departmentRepository.DeleteDepartmentAsync(department);
        await _logRepository.AddDepartmentLogForCurrentActor(
            department,
            Action.REMOVED_DEPARTMENT,
            []
        );
        await _unitOfWork.CompleteAsync();
    }
}
