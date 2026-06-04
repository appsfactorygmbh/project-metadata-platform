using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Query to get all departments.
/// </summary>
public record GetAllDepartmentsQuery : IRequest<IEnumerable<Department>>;
