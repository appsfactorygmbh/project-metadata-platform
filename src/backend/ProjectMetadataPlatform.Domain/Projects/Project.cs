using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Domain.Projects;

/// <summary>
/// The representation of a project in the Database.
/// </summary>
public class Project
{
    /// <summary>
    /// Gets or sets the id of the project.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the project. This property is required.
    /// </summary>
    public required string ProjectName { get; set; }

    /// <summary>
    /// Gets or sets the short version of the ProjectName that can be used to identify a Project in requests instead of the Id. This property is required.
    /// </summary>
    public required string Slug { get; set; }

    /// <summary>
    /// Gets or sets the name of the client associated with the project. This property is required.
    /// </summary>
    public required string ClientName { get; set; }

    /// <summary>
    /// Is used for the many-to-many relation in EF core.
    /// </summary>
    public ICollection<ProjectPlugins>? ProjectPlugins { get; set; }

    /// <summary>
    /// Includes the logs for the project. Used for one to many relationship.
    /// </summary>
    public ICollection<Log>? Logs { get; set; }

    /// <summary>
    /// The team that is responsible for the project. It is a many to one relationship. A project has one team. A team can work on multiple projects.
    /// </summary>
    public Team? Team { get; set; }

    /// <summary>
    /// The team id of the responsible team. Used for many to one relationship.
    /// </summary>
    public int? TeamId { get; set; }

    /// <summary>
    /// A boolean indicating if a plugin is archived/deleted.
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// Internal id of the offer associated with the project.
    /// </summary>
    public string? OfferId { get; set; }

    /// <summary>
    /// The company that is responsible for the project.
    /// </summary>
    public string Company { get; set; } = "";

    /// <summary>
    /// The state of the company.
    /// </summary>
    public CompanyState CompanyState { get; set; }

    /// <summary>
    /// The security level of the project.
    /// </summary>
    public SecurityLevel IsmsLevel { get; set; }

    /// <summary>
    /// Notes or additional information about the project.
    /// </summary>
    public string Notes { get; set; } = "";
}
