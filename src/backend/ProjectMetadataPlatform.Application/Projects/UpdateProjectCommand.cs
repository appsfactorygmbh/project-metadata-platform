using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Command to update a project with the given attributes.
/// </summary>
/// <param name="ProjectName">Name of the project</param>
/// <param name="ClientName">Name of the client</param>
/// <param name="OfferId">Id of the offer associated with project.</param>
/// <param name="Company">Company responsible for project.</param>
/// <param name="CompanyState">State of company.</param>
/// <param name="TeamId">The id of the team associated with the project.</param>
/// <param name="IsmsLevel">Security Level of project.</param>
/// <param name="Id">Id of the project</param>
/// <param name="Plugins">List of plugins associated with the project</param>
/// <param name="IsArchived">Indicates if the project is archived</param>
/// <param name="Notes">Additional Project Notes</param>
public record UpdateProjectCommand(
    int Id,
    string ProjectName,
    string ClientName,
    string? OfferId,
    string Company,
    CompanyState CompanyState,
    int? TeamId,
    SecurityLevel IsmsLevel,
    List<ProjectPlugins> Plugins,
    bool IsArchived,
    string Notes
) : IRequest<int>;
