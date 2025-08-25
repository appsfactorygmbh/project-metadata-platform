using ProjectMetadataPlatform.Api.Teams.Models;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Api.Projects.Models;

/// <summary>
/// Represents a response for the GetProjects API call.
/// </summary>
/// <param name="Id">The id of the project.</param>
/// <param name="Slug">The Slug for the project name.</param>
/// <param name="ProjectName">The name of the project.</param>
/// <param name="ClientName">The name of the client associated with the project.</param>
/// <param name="IsArchived">If the project is archived or not.</param>
/// <param name="Company">The company associated with the project.</param>
/// <param name="Team">The team working on the project. Optional.</param>
/// <param name="IsmsLevel">The ISMS (Information Security Management System) level of the project.</param>
/// <param name="Notes">Additonal Notes on the project</param>
public record GetProjectsResponse(
    int Id,
    string Slug,
    string ProjectName,
    string ClientName,
    string Company,
    bool IsArchived,
    GetTeamResponse? Team,
    SecurityLevel IsmsLevel,
    string Notes
);
