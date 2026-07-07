using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Handler for the <see cref="GetAllDepartmentsQuery" />.
/// </summary>
public class GetAllDepartmentsQueryHandler
    : IRequestHandler<GetAllDepartmentsQuery, IEnumerable<Department>>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllDepartmentsQueryHandler" />.
    /// </summary>
    public GetAllDepartmentsQueryHandler(
        IDepartmentRepository departmentRepository,
        IAuthorizationService authorizationService
    )
    {
        _departmentRepository = departmentRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles a Request to return all Departments.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Departments.</returns>
    public async Task<IEnumerable<Department>> Handle(
        GetAllDepartmentsQuery request,
        CancellationToken cancellationToken
    )
    {
        var departments = await _departmentRepository.GetDepartmentsAsync();
        var queriedDepartments = await _authorizationService.TryGetPlanResourceQuery(departments);
        if (queriedDepartments == null)
        {
            List<Department> filteredDepartments = [];
            foreach (var department in departments)
            {
                if (
                    (
                        await _authorizationService.CheckAccess(
                            department,
                            [AuthorizationConstants.Actions.GET]
                        )
                    )[AuthorizationConstants.Actions.GET]
                )
                {
                    filteredDepartments.Add(department);
                }
            }
            return filteredDepartments.OrderBy(department =>
                department.DepartmentName.ToLowerInvariant()
            );
        }
        return (await queriedDepartments.ToListAsync(cancellationToken: cancellationToken)).OrderBy(
            department => department.DepartmentName.ToLowerInvariant()
        );
    }
}
