using System;
using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.OfficeLocations;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Domain.Logs;

/// <summary>
/// Representation of a Log in database.
/// </summary>
public class Log
{
    /// <summary>
    /// The id of the Log
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the associated user.
    /// </summary>
    public ApplicationUser? Author { get; set; }

    /// <summary>
    /// Gets or sets the associated token.
    /// </summary>
    public ApiToken? AuthorToken { get; set; }

    /// <summary>
    /// Gets or sets the ID of the author user.
    /// </summary>
    public required string? AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the author token.
    /// </summary>
    public required int? AuthorTokenId { get; set; }

    /// <summary>
    /// The Name of the user or token taking action.
    /// </summary>
    public required string? AuthorName { get; set; }

    /// <summary>
    /// The TImeStamp when the action was taken
    /// </summary>
    public DateTimeOffset TimeStamp { get; set; }

    /// <summary>
    /// The Project, on wich the action was taken
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    /// The Project Id of the related Project
    /// </summary>
    public int? ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    public string? ProjectName { get; set; }

    /// <summary>
    /// Gets or sets the global plugin associated with the log.
    /// </summary>
    public Plugin? GlobalPlugin { get; set; }

    /// <summary>
    /// Gets or sets the ID of the global plugin.
    /// </summary>
    public int? GlobalPluginId { get; set; }

    /// <summary>
    /// Gets or sets the name of the global plugin.
    /// </summary>
    public string? GlobalPluginName { get; set; }

    /// <summary>
    /// Gets or sets the affected user.
    /// </summary>
    public ApplicationUser? AffectedUser { get; set; }

    /// <summary>
    /// Gets or sets the ID of the affected user.
    /// </summary>
    public string? AffectedUserId { get; set; }

    /// <summary>
    /// Gets or sets the email of the affected user.
    /// </summary>
    public string? AffectedUserEmail { get; set; }

    /// <summary>
    /// The Team, on which the action was taken.
    /// </summary>
    public Team? Team { get; set; }

    /// <summary>
    /// The Team id of the related project.
    /// </summary>
    public int? TeamId { get; set; }

    /// <summary>
    /// The Team name of the related project.
    /// </summary>
    public string? TeamName { get; set; }

    /// <summary>
    /// The Token, on which the action was taken.
    /// </summary>
    public ApiToken? AffectedToken { get; set; }

    /// <summary>
    /// The id of the related token.
    /// </summary>
    public int? AffectedTokenId { get; set; }

    /// <summary>
    /// The id of the related token.
    /// </summary>
    public string? AffectedTokenName { get; set; }

    /// <summary>
    /// The Company, on which the action was taken.
    /// </summary>
    public Company? Company { get; set; }

    /// <summary>
    /// The Company id of the related Company.
    /// </summary>
    public int? CompanyId { get; set; }

    /// <summary>
    /// The Company name of the related Company.
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// The Department, on which the action was taken.
    /// </summary>
    public Department? Department { get; set; }

    /// <summary>
    /// The Department id of the related Department.
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// The Department name of the related Department.
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// The BusinessUnit, on which the action was taken.
    /// </summary>
    public BusinessUnit? BusinessUnit { get; set; }

    /// <summary>
    /// The BusinessUnit id of the related BusinessUnit.
    /// </summary>
    public int? BusinessUnitId { get; set; }

    /// <summary>
    /// The BusinessUnit name of the related BusinessUnit.
    /// </summary>
    public string? BusinessUnitName { get; set; }

    /// <summary>
    /// The Office Location, on which the action was taken.
    /// </summary>
    public OfficeLocation? OfficeLocation { get; set; }

    /// <summary>
    /// The Office Location id of the related Office Location.
    /// </summary>
    public int? OfficeLocationId { get; set; }

    /// <summary>
    /// The Office Location name of the related Office Location.
    /// </summary>
    public string? OfficeLocationName { get; set; }

    /// <summary>
    /// The taken action
    /// </summary>
    public Action Action { get; set; }

    /// <summary>
    /// The changes that were made.
    /// </summary>
    public List<LogChange>? Changes { get; set; }
}
