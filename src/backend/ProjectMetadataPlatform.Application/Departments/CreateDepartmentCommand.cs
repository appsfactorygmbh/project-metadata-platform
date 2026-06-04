using MediatR;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Command to create a new department.
/// </summary>
/// <param name="DepartmentName">Name of the new department.</param>
public record CreateDepartmentCommand(string DepartmentName) : IRequest<int>;
