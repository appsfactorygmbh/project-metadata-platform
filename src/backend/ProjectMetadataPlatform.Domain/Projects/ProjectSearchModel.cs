namespace ProjectMetadataPlatform.Domain.Projects;

/// <summary>
/// Record representing a materialized Search View for efficiently querying Projects from the database.
/// </summary>
public class ProjectSearchModel
{
    /// <summary>
    /// Id of the Project. Primary Key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Slug of the Project.
    /// </summary>
    public required string Slug { get; set; }

    /// <summary>
    /// Name of the Project.
    /// </summary>
    public required string ProjectName { get; set; }

    /// <summary>
    /// Client Name of the Project.
    /// </summary>
    public required string ClientName { get; set; }

    /// <summary>
    /// Whether the project is archived.
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// Whether the project is an engineer on call project.
    /// </summary>
    public bool IsEoC { get; set; }

    /// <summary>
    /// Notes on the Project.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// String Representation of the Projects Security Level Enum Value.
    /// </summary>
    public required string SecurityLevelText { get; set; }

    /// <summary>
    /// String Representation of the Projects Company State Enum Value.
    /// </summary>
    public required string CompanyStateText { get; set; }

    /// <summary>
    /// Name of the related Company.
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Name of the related Team.
    /// </summary>
    public string? TeamName { get; set; }

    /// <summary>
    /// Name of the Ptl of the related Team
    /// </summary>
    public string? PTL { get; set; }

    /// <summary>
    /// Name of the related Business Unit of the related Team.
    /// </summary>
    public string? BusinessUnitName { get; set; }

    /// <summary>
    /// String containing all information of all Plugins of the Project
    /// </summary>
    public required string PluginsSearchText { get; set; }

    /// <summary>
    /// String containing all information of all PluginBilling objects of the Project
    /// </summary>
    public required string BillingSearchText { get; set; }
}
