namespace ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

/// <summary>
/// Exception thrown when an invalid refresh token is provided.
/// </summary>
public class AuthInvalidRefreshTokenException() : AuthException("Invalid refresh token.");
