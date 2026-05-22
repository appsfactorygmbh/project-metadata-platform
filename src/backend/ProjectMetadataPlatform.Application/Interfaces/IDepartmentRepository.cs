using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface IDepartmentRepository
{
    Task<bool> CheckIfDepartmentNameExistsAsync(string departmentName);

    Task<Department> GetDepartmentByNameAsync(string departmentName);

    Task AddDepartmentAsync(Department department);
}

