namespace ProjectMetadataPlatform.Domain.Auth;

/// <summary>
/// Enum representing different scopes for api tokens.
/// </summary>
public enum TokenScopes
{
    //TODO: Keep up to date with the API and if necessary make manual migration changes.

    /// <summary>
    /// Scope for a Scim Token (Gives Access to all User Endpoints).
    /// </summary>
    SCIM,

    /// <summary>
    /// Scope for returning Projects.
    /// </summary>
    GET_PROJECT,

    /// <summary>
    /// Scope for creating Projects.
    /// </summary>
    CREATE_PROJECT,

    /// <summary>
    /// Scope for updating Projects.
    /// </summary>
    EDIT_PROJECT,

    /// <summary>
    /// Scope for deleting Projects.
    /// </summary>
    DELETE_PROJECT,

    /// <summary>
    /// Scope for returning Global Plugins.
    /// </summary>
    GET_PLUGIN,

    /// <summary>
    /// Scope for creating Global Plugins.
    /// </summary>
    CREATE_PLUGIN,

    /// <summary>
    /// Scope for updating Global Plugins.
    /// </summary>
    EDIT_PLUGIN,

    /// <summary>
    /// Scope for deleting Global Plugins.
    /// </summary>
    DELETE_PLUGIN,

    /// <summary>
    /// Scope for reading Users.
    /// </summary>
    GET_APPLICATIONUSER,

    /// <summary>
    /// Scope for reading Teams.
    /// </summary>
    GET_TEAM,

    /// <summary>
    /// Scope for creating Teams,
    /// </summary>
    CREATE_TEAM,

    /// <summary>
    /// Scope for updating Teams.
    /// </summary>
    EDIT_TEAM,

    /// <summary>
    /// Scope for deleting Teams.
    /// </summary>
    DELETE_TEAM,

    /// <summary>
    /// Scope for reading Companies.
    /// </summary>
    GET_COMPANY,

    /// <summary>
    /// Scope for creating Companies.
    /// </summary>
    CREATE_COMPANY,

    /// <summary>
    /// Scope for editing Companies.
    /// </summary>
    EDIT_COMPANY,

    /// <summary>
    /// Scope for deleting Companies.
    /// </summary>
    DELETE_COMPANY,

    /// <summary>
    /// Scope for reading Departments.
    /// </summary>
    GET_DEPARTMENT,

    /// <summary>
    /// Scope for creating Departments.
    /// </summary>
    CREATE_DEPARTMENT,

    /// <summary>
    /// Scope for editing Departments.
    /// </summary>
    EDIT_DEPARTMENT,

    /// <summary>
    /// Scope for deleting Departments.
    /// </summary>
    DELETE_DEPARTMENT,

    /// <summary>
    /// Scope for reading BUs.
    /// </summary>
    GET_BUSINESSUNIT,

    /// <summary>
    /// Scope for creating BUs.
    /// </summary>
    CREATE_BUSINESSUNIT,

    /// <summary>
    /// Scope for editing BUs.
    /// </summary>
    EDIT_BUSINESSUNIT,

    /// <summary>
    /// Scope for deleting BUs.
    /// </summary>
    DELETE_BUSINESSUNIT,

    /// <summary>
    /// Scope for reading Office Locations.
    /// </summary>
    GET_OFFICELOCATION,

    /// <summary>
    /// Scope for creating Office Locations.
    /// </summary>
    CREATE_OFFICELOCATION,

    /// <summary>
    /// Scope for updating Office Locations.
    /// </summary>
    EDIT_OFFICELOCATION,

    /// <summary>
    /// Scope for deleting Office Locations.
    /// </summary>
    DELETE_OFFICELOCATION,

    /// <summary>
    /// Scope for reading Logs.
    /// </summary>
    GET_LOG,

    /// <summary>
    /// Scope for reading plugins of a project.
    /// </summary>
    GET_PROJECTPLUGIN,

    /// <summary>
    /// Scope for adding plugins to a project.
    /// </summary>
    CREATE_PROJECTPLUGIN,

    /// <summary>
    /// Scope for editing a plugin of a project.
    /// </summary>
    EDIT_PROJECTPLUGIN,

    /// <summary>
    /// Scope for removing a plugin from a project.
    /// </summary>
    DELETE_PROJECTPLUGIN,

    /// <summary>
    /// Scope for reading global billing objects.
    /// </summary>
    GET_GLOBALBILLING,

    /// <summary>
    /// Scope for creating a global billing object.
    /// </summary>
    CREATE_GLOBALBILLING,

    /// <summary>
    /// Scope for editing a global billing object.
    /// </summary>
    EDIT_GLOBALBILLING,

    /// <summary>
    /// Scope for deleting a global billing object.
    /// </summary>
    DELETE_GLOBALBILLING,

    /// <summary>
    /// Scope for reading billing information of a plugin of a project.
    /// </summary>
    GET_PLUGINBILLING,

    /// <summary>
    /// Scope for adding billing information to a plugin of a project.
    /// </summary>
    CREATE_PLUGINBILLING,

    /// <summary>
    /// Scope for editing billing information of a plugin of a project.
    /// </summary>
    EDIT_PLUGINBILLING,

    /// <summary>
    /// Scope for removing billing information from a plugin of a project.
    /// </summary>
    DELETE_PLUGINBILLING,
}
