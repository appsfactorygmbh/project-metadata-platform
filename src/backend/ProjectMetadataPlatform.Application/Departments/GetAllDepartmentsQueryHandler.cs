using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Handler for the <see cref="GetAllDepartmentsQuery" />.
/// </summary>
public class GetAllDepartmentsQueryHandler
    : IRequestHandler<
        GetAllDepartmentsQuery,
        (IEnumerable<Department>, IEnumerable<AuthorizationConstants.Actions>, DepartmentCursor?)
    >
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IPaginationHelper _paginationHelper;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllDepartmentsQueryHandler" />.
    /// </summary>
    public GetAllDepartmentsQueryHandler(
        IDepartmentRepository departmentRepository,
        IAuthorizationService authorizationService,
        IPaginationHelper paginationHelper
    )
    {
        _departmentRepository = departmentRepository;
        _authorizationService = authorizationService;
        _paginationHelper = paginationHelper;
    }

    /// <summary>
    /// Handles a Request to return all Departments.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Departments and allowed actions.</returns>
    public async Task<(
        IEnumerable<Department>,
        IEnumerable<AuthorizationConstants.Actions>,
        DepartmentCursor?
    )> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var departments = await _departmentRepository.GetDepartmentsAsync(request.Cursor);
        var (paginatedDepartments, lastDepartment) = await _paginationHelper.PaginateWithAuthAsync(
            departments,
            request.Limit
        );
        var permissions = await _authorizationService.GetAllowedActions<Department>(
            actions: [AuthorizationConstants.Actions.CREATE]
        );
        var nextCursor =
            lastDepartment == null ? null : new DepartmentCursor(lastDepartment.DepartmentName);
        return (paginatedDepartments, permissions, nextCursor);
    }
}
