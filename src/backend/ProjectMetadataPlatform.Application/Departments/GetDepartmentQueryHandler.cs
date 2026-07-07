using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Handler for the <see cref="GetDepartmentQuery" />.
/// </summary>
public class GetDepartmentQueryHandler : IRequestHandler<GetDepartmentQuery, Department>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetDepartmentQueryHandler" />.
    /// </summary>
    public GetDepartmentQueryHandler(
        IDepartmentRepository departmentRepository,
        IAuthorizationService authorizationService
    )
    {
        _departmentRepository = departmentRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles a Query for returning a department.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A department.</returns>
    public async Task<Department> Handle(
        GetDepartmentQuery request,
        CancellationToken cancellationToken
    )
    {
        var department = await _departmentRepository.GetDepartmentAsync(request.Id);
        if (
            !(
                await _authorizationService.CheckAccess(
                    department,
                    [AuthorizationConstants.Actions.GET]
                )
            )[AuthorizationConstants.Actions.GET]
        )
        {
            throw new UnauthorizedException();
        }
        return department;
    }
}
