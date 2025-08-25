namespace ProjectMetadataPlatform.Domain.Auth;

/// <summary>
/// Holds the jwt access Token and refresh token.
/// </summary>
public class JwtTokens
{
    /// <summary>
    /// Access Token of the User.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Refresh Token of the User.
    /// </summary>
    public string? RefreshToken { get; set; }
}
