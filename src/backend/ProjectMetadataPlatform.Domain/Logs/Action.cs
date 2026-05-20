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

    /// <summary>
    /// Represents the action of adding an api token.
    /// </summary>
    ADDED_API_TOKEN,

    /// <summary>
    /// Represents the action of removing an api token.
    /// </summary>
    REMOVED_API_TOKEN,

    /// <summary>
    ///  Represents the action of regenerating an api token value.
    /// </summary>
    REGENERATED_API_TOKEN,

    /// <summary>
    /// Represents the action of adding a company.
    /// </summary>
    ADDED_COMPANY,

    /// <summary>
    /// Represents the action of updating a company.
    /// </summary>
    UPDATED_COMPANY,

    /// <summary>
    /// Represents the action of removing a company.
    /// </summary>
    REMOVED_COMPANY,

    /// <summary>
    /// Represents the action of adding a department.
    /// </summary>
    ADDED_DEPARTMENT,

    /// <summary>
    /// Represents the action of updating a department.
    /// </summary>
    UPDATED_DEPARTMENT,

    /// <summary>
    /// Represents the action of removing a department.
    /// </summary>
    REMOVED_DEPARTMENT,

    /// <summary>
    /// Represents the action of adding a business unit.
    /// </summary>
    ADDED_BUSINESS_UNIT,

    /// <summary>
    /// Represents the action of updating a business unit.
    /// </summary>
    UPDATED_BUSINESS_UNIT,

    /// <summary>
    /// Represents the action of removing a business unit.
    /// </summary>
    REMOVED_BUSINESS_UNIT,

    /// <summary>
    /// Represents the action of adding a office location.
    /// </summary>
    ADDED_OFFICE_LOCATION,

    /// <summary>
    /// Represents the action of updating a office location.
    /// </summary>
    UPDATED_OFFICE_LOCATION,

    /// <summary>
    /// Represents the action of removing a office location.
    /// </summary>
    REMOVED_OFFICE_LOCATION,
}
