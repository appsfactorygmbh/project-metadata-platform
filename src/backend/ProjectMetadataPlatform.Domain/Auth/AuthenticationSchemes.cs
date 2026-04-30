namespace ProjectMetadataPlatform.Domain.Auth;

/// <summary>
/// Defines the names of availables auth schemes.
/// </summary>
public static class AuthenticationSchemes
{
    /// <summary>
    /// Scheme for authenticating tokens created from basic login.
    /// </summary>
    public const string BASIC = "Basic";

    /// <summary>
    /// Scheme for authenticating tokens from entra oidc.
    /// </summary>
    public const string AZURE = "Azure";

    /// <summary>
    /// Scheme for authenticating custom bearer tokens.
    /// </summary>
    public const string API_TOKEN = "ApiToken";

    /// <summary>
    /// Scheme for automatically selecting the fitting authentication scheme for the token.
    /// </summary>
    public const string SELECTOR = "Selector";
}
