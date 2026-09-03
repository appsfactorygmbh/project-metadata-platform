using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Command to create a new project with the given attributes.
/// </summary>
/// <param name="ProjectName">Name of the project</param>
/// <param name="ClientName">Name of the client</param>
/// <param name="CompanyId">Id of the Company responsible for project.</param>
/// <param name="CompanyState">State of company.</param>
/// <param name="TeamId">The id of the team associated with the project.</param>
/// <param name="IsmsLevel">Security Level of project.</param>
/// <param name="IsEoC">If the project is an Engineer on Call project.</param>
/// <param name="Notes">Additonal Project Notes</param>
public record CreateProjectCommand(
    string ProjectName,
    string ClientName,
    int CompanyId,
    CompanyState CompanyState,
    int? TeamId,
    SecurityLevel IsmsLevel,
    bool IsEoC,
    string Notes
) : IRequest<int>;
