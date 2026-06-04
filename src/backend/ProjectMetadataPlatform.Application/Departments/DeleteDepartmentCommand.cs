using MediatR;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Command to delete a department.
/// </summary>
/// <param name="Id">Id of the deleted department.</param>
public record DeleteDepartmentCommand(int Id) : IRequest;
