using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Query to get all departments.
/// </summary>
public record GetAllDepartmentsQuery
    : IRequest<(IEnumerable<Department>, IEnumerable<AuthorizationConstants.Actions>)>;
