using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for Departments
/// </summary>
public interface IDepartmentRepository
{
    /// <summary>
    /// Returns all Departments.
    /// </summary>
    /// <returns>List of all Departments.</returns>
    Task<IList<Department>> GetDepartmentsAsync();

    /// <summary>
    /// Returns a Department by its Id.
    /// </summary>
    /// <param name="id">Id of the Department.</param>
    /// <returns>Specified Department</returns>
    Task<Department> GetDepartmentAsync(int id);

    /// <summary>
    /// Checks wether a Departments with the specified Id exists.
    /// </summary>
    /// <param name="id">Id that gets checked.</param>
    /// <returns>Boolean representing the Existence of the Department</returns>
    Task<bool> CheckIfDepartmentExistsAsync(int id);

    /// <summary>
    /// Checks wether a Departments with the specified Name exists.
    /// </summary>
    /// <param name="departmentName">Name that gets checked.</param>
    /// <returns>Boolean representing the Existence of the Department</returns>
    Task<bool> CheckIfDepartmentNameExistsAsync(string departmentName);

    /// <summary>
    /// Returns a Department by its Name.
    /// </summary>
    /// <param name="departmentName">Name of the Department.</param>
    /// <returns>Specified Department</returns>
    Task<Department> GetDepartmentByNameAsync(string departmentName);

    /// <summary>
    /// Adds a new Department.
    /// </summary>
    /// <param name="department">new Department to be added.</param>
    /// <returns></returns>
    Task AddDepartmentAsync(Department department);

    /// <summary>
    /// Updates an existing department.
    /// </summary>
    /// <param name="department">Updated Department.</param>
    /// <returns>Updated Department.</returns>
    Task<Department> UpdateDepartmentAsync(Department department);

    /// <summary>
    /// Deletes a Department.
    /// </summary>
    /// <param name="department">Department to be deleted.</param>
    /// <returns>Deleted Department.</returns>
    Task<Department> DeleteDepartmentAsync(Department department);
}
