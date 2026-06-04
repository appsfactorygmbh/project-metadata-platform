using MediatR;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Query to return a specified department.
/// </summary>
/// <param name="Id">Id of the department.</param>
public record GetDepartmentQuery(int Id) : IRequest<Department>;
