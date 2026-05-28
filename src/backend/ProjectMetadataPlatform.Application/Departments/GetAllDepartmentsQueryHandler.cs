using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Handler for the <see cref="GetAllDepartmentsQuery" />.
/// </summary>
public class GetAllDepartmentsQueryHandler
    : IRequestHandler<GetAllDepartmentsQuery, IEnumerable<Department>>
{
    private readonly IDepartmentRepository _departmentRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllDepartmentsQueryHandler" />.
    /// </summary>
    public GetAllDepartmentsQueryHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
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
        return departments.OrderBy(department => department.DepartmentName.ToLowerInvariant());
    }
}
