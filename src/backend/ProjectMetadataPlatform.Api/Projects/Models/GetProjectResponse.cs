using ProjectMetadataPlatform.Api.Teams.Models;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Api.Projects.Models;

/// <summary>
/// Represents a response to the GetProject API call.
/// </summary>
/// <param name="ProjectName"></param>
/// <param name="ClientName"></param>
/// <param name="Id">The identification number for the project.</param>
/// <param name="Slug">The Slug for the project name.</param>
/// <param name="IsArchived">If the project is archived or not.</param>
/// <param name="OfferId">Internal id of the offer associated with the project.</param>
/// <param name="Company">The company that is responsible for the project.</param>
/// <param name="Team">The team working on the project. Optional.</param>
/// <param name="CompanyState">The state of the company. (INTERNAL or EXTERNAL)</param>
/// <param name="IsmsLevel">The security level of the project (NORMAL, HIGH, VERY_HIGH)</param>
/// <param name="Notes">Additonal Notes on the project</param>
public record GetProjectResponse(
    int Id,
    string Slug,
    string ProjectName,
    string ClientName,
    string? OfferId,
    string Company,
    bool IsArchived,
    GetTeamResponse? Team,
    CompanyState CompanyState,
    SecurityLevel IsmsLevel,
    string Notes
);
