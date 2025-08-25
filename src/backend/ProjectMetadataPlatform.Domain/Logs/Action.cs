namespace ProjectMetadataPlatform.Domain.Logs;

/// <summary>
///     Enum for log changes actions
/// </summary>
public enum Action
{
    // TODO keep in sync with:
    // - LogRepository in the Infrastructure project (ActionMessages and ActionWhiteLists)
    // - the LogConverter in the Api project

    /// <summary>
    /// Represents the action of adding a project.
    /// </summary>
    ADDED_PROJECT,

    /// <summary>
    /// Represents the action of adding a project plugin.
    /// </summary>
    ADDED_PROJECT_PLUGIN,

    /// <summary>
    /// Represents the action of updating a project.
    /// </summary>
    UPDATED_PROJECT,

    /// <summary>
    /// Represents the action of updating a project plugin.
    /// </summary>
    UPDATED_PROJECT_PLUGIN,

    /// <summary>
    /// Represents the action of removing a project plugin.
    /// </summary>
    REMOVED_PROJECT_PLUGIN,

    /// <summary>
    /// Represents the action of archiving a project.
    /// </summary>
    ARCHIVED_PROJECT,

    /// <summary>
    /// Represents the action of unarchiving a project.
    /// </summary>
    UNARCHIVED_PROJECT,

    /// <summary>
    /// Represents the action of adding a user.
    /// </summary>
    ADDED_USER,

    /// <summary>
    /// Represents the action of updating a user.
    /// </summary>
    UPDATED_USER,

    /// <summary>
    /// Represents the action of removing a user.
    /// </summary>
    REMOVED_USER,

    /// <summary>
    /// Represents the action of removing a project.
    /// </summary>
    REMOVED_PROJECT,

    /// <summary>
    /// Represents the action of adding a global plugin.
    /// </summary>
    ADDED_GLOBAL_PLUGIN,

    /// <summary>
    /// Represents the action of updating a global plugin.
    /// </summary>
    UPDATED_GLOBAL_PLUGIN,

    /// <summary>
    /// Represents the action of archiving a global plugin.
    /// </summary>
    ARCHIVED_GLOBAL_PLUGIN,

    /// <summary>
    /// Represents the action of unarchiving a global plugin.
    /// </summary>
    UNARCHIVED_GLOBAL_PLUGIN,

    /// <summary>
    /// Represents the action of removing a global plugin.
    /// </summary>
    REMOVED_GLOBAL_PLUGIN,

    /// <summary>
    /// Represents the action of adding a team.
    /// </summary>
    ADDED_TEAM,

    /// <summary>
    /// Represents the action of updating a team.
    /// </summary>
    UPDATED_TEAM,

    /// <summary>
    /// Represents the action of removing a team.
    /// </summary>
    REMOVED_TEAM,
}
