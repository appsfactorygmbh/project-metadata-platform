namespace ProjectMetadataPlatform.Domain.Auth;

/// <summary>
/// Enum representing different scopes for api tokens.
/// </summary>
public enum TokenScopes
{
    //TODO: Keep up to date with the API and if necessary make manual migration changes.

    /// <summary>
    /// Scope for a Scim Token.
    /// </summary>
    SCIM,

    GET_PROJECT,

    CREATE_PROJECT,

    EDIT_PROJECT,

    DELETE_PROJECT,

    GET_PLUGIN,

    CREATE_PLUGIN,

    EDIT_PLUGIN,

    DELETE_PLUGIN,

    GET_APPLICATIONUSER,

    GET_TEAM,

    CREATE_TEAM,

    EDIT_TEAM,

    DELETE_TEAM,

    GET_COMPANY,

    CREATE_COMPANY,

    EDIT_COMPANY,

    DELETE_COMPANY,

    GET_DEPARTMENT,

    CREATE_DEPARTMENT,

    EDIT_DEPARTMENT,

    DELETE_DEPARTMENT,

    GET_BUSINESSUNIT,

    CREATE_BUSINESSUNIT,

    EDIT_BUSINESSUNIT,

    DELETE_BUSINESSUNIT,

    GET_OFFICELOCATION,

    CREATE_OFFICELOCATION,

    EDIT_OFFICELOCATION,

    DELETE_OFFICELOCATION,
}
