using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Represents a request to filter projects based on various criteria.
/// </summary>
/// <param name="ProjectName">Optional. The name of the project to filter by.</param>
/// <param name="ClientName">Optional. The name of the client associated with the project to filter by.</param>
/// <param name="BusinessUnit">Optional. A list of business units to filter the projects by.</param>
/// <param name="TeamName">Optional. A list of team names to filter the projects by.</param>
/// <param name="IsArchived">Optional. The archival status of the projects to filter by.</param>
/// <param name="Company">Optional. A list of companies to filter the projects by.</param>
/// <param name="IsmsLevel">Optional. The ISMS level to filter the projects by.</param>
public record ProjectFilterRequest(
    string? ProjectName,
    string? ClientName,
    List<string>? BusinessUnit,
    List<string>? TeamName,
    bool? IsArchived,
    List<string>? Company,
    SecurityLevel? IsmsLevel
);
