using MediatR;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Command for updating a department.
/// </summary>
/// <param name="Id">Id of the department.</param>
/// <param name="DepartmentName">Name Name for the department.</param>
public record UpdateDepartmentCommand(int Id, string? DepartmentName = null) : IRequest<Department>;
