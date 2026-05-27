using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Departments;

public class GetDepartmentQueryHandler : IRequestHandler<GetDepartmentQuery, Department>
{
    private readonly IDepartmentRepository _departmentRepository;

    public GetDepartmentQueryHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<Department> Handle(
        GetDepartmentQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _departmentRepository.GetDepartmentAsync(request.Id);
    }
}
