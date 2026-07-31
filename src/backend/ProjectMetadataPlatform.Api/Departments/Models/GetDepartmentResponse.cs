using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.Departments.Models;

/// <summary>
/// Represents a Response containing a single department.
/// </summary>
/// <param name="Id">Id of the department.</param>
/// <param name="DepartmentName">Name of the department.</param>
/// <param name="Permissions">Permissions on the department.</param>
public record GetDepartmentResponse(
    int Id,
    string DepartmentName,
    List<AuthorizationConstants.Actions>? Permissions = null
);
