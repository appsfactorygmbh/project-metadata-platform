using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Handler for the <see cref="GetDepartmentQuery" />.
/// </summary>
public class GetDepartmentQueryHandler : IRequestHandler<GetDepartmentQuery, Department>
{
    private readonly IDepartmentRepository _departmentRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetDepartmentQueryHandler" />.
    /// </summary>
    public GetDepartmentQueryHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
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
        return await _departmentRepository.GetDepartmentAsync(request.Id);
    }
}
