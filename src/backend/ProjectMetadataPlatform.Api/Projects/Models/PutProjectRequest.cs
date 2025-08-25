using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Api.Projects.Models;

/// <summary>
/// Represents a request to create a new / update an existing project.
/// </summary>
/// <param name="ProjectName">The name of the project.</param>
/// <param name="ClientName">The name of the client for the project.</param>
/// <param name="OfferId">Id of the offer associated with project.</param>
/// <param name="Company">Company responsible for project.</param>
/// <param name="TeamId">The id of the team that should be assigned to the project.</param>
/// <param name="CompanyState">State of company.</param>
/// <param name="IsmsLevel">Security Level of project.</param>
/// <param name="Notes">Additonal Notes on the project</param>
/// <param name="PluginList">An optional list of plugins associated to the project.</param>
/// <param name="IsArchived">Indicates if the project is archived.</param>
public record PutProjectRequest(
    string ProjectName,
    string ClientName,
    string? OfferId,
    string Company,
    int? TeamId,
    CompanyState CompanyState,
    SecurityLevel IsmsLevel,
    string Notes,
    List<UpdateProjectPluginRequest>? PluginList = null,
    bool IsArchived = false
);
