using System;
using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Domain.Logs;

/// <summary>
///     Representation of a Log in database.
/// </summary>
public class Log
{
    /// <summary>
    ///     The id of the Log
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the associated user.
    /// </summary>
    public User.User? Author { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user.
    /// </summary>
    public required string? AuthorId { get; set; }

    /// <summary>
    ///     the Email of the user taking action
    /// </summary>
    public required string? AuthorEmail { get; set; }

    /// <summary>
    ///     The TImeStamp when the action was taken
    /// </summary>
    public DateTimeOffset TimeStamp { get; set; }

    /// <summary>
    ///     The Project, on wich the action was taken
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    ///     The Project Id of the related Project
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
    public User.User? AffectedUser { get; set; }

    /// <summary>
    /// Gets or sets the ID of the affected user.
    /// </summary>
    public string? AffectedUserId { get; set; }

    /// <summary>
    /// Gets or sets the email of the affected user.
    /// </summary>
    public string? AffectedUserEmail { get; set; }

    /// <summary>
    ///     The taken action
    /// </summary>
    public Action Action { get; set; }

    /// <summary>
    ///     The changes that were made.
    /// </summary>
    public List<LogChange>? Changes { get; set; }
}
