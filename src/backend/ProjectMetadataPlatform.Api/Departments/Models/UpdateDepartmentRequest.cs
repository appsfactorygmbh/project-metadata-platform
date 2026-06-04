namespace ProjectMetadataPlatform.Api.Departments.Models;

/// <summary>
/// Represents a Request to update a department.
/// </summary>
/// <param name="DepartmentName">New Name for the department.</param>
public record UpdateDepartmentRequest(string? DepartmentName = null);
