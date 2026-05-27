using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface IDepartmentRepository
{
    Task<IList<Department>> GetDepartmentsAsync();
    Task<Department> GetDepartmentAsync(int id);
    Task<bool> CheckIfDepartmentNameExistsAsync(string departmentName);
    Task<bool> CheckIfDepartmentExistsAsync(int id);
    Task<Department> GetDepartmentByNameAsync(string departmentName);

    Task AddDepartmentAsync(Department department);

    Task<Department> UpdateDepartmentAsync(Department department);
    Task<Department> DeleteDepartmentAsync(Department department);
}
