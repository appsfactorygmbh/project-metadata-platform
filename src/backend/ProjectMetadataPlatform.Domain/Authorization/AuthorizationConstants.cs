namespace ProjectMetadataPlatform.Domain.Authorization;

/// <summary>
/// Class Containing Constants for Authorization
/// </summary>
public static class AuthorizationConstants
{
    /// <summary>
    /// Authorization Service API Version.
    /// </summary>
    public const string API_VERSION = "api.cerbos.dev/v1";

    /// <summary>
    /// Authorization Policy Version.
    /// </summary>
    public const string POLICY_VERSION = "default";

    /// <summary>
    /// Name of User Principle.
    /// </summary>
    public const string PRINCIPLE_USER = "User";

    /// <summary>
    /// Name of Token Principle.
    /// </summary>
    public const string PRINCIPLE_TOKEN = "ApiToken";

    //TODO: Keep up to date with the API
    /// <summary>
    /// Enum for Authorization Request Actions.
    /// </summary>
    public enum Actions
    {
        /// <summary>
        /// Action for reading Resources.
        /// </summary>
        GET,

        /// <summary>
        /// Action for creating Resources.
        /// </summary>
        CREATE,

        /// <summary>
        /// Action for edting a Resource.
        /// </summary>
        EDIT,

        /// <summary>
        /// Action for deleting Resources.
        /// </summary>
        DELETE,
    }
}
