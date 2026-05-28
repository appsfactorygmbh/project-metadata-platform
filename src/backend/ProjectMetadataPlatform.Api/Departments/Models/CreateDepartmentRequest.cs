namespace ProjectMetadataPlatform.Api.Departments.Models;

/// <summary>
/// Represents a Request for creating a new Department.
/// </summary>
/// <param name="DepartmentName">Name of the new department. </param>
public record CreateDepartmentRequest(string DepartmentName);
