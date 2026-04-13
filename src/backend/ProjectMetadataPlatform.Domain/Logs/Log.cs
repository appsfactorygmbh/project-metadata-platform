using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Domain.Auth;
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
    /// The taken action
    /// </summary>
    public Action Action { get; set; }

    /// <summary>
    /// The changes that were made.
    /// </summary>
    public List<LogChange>? Changes { get; set; }
}
