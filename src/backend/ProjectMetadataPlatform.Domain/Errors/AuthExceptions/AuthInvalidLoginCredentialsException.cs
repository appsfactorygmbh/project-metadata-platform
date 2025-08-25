namespace ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

/// <summary>
/// Exception thrown when invalid login credentials are provided.
/// </summary>
public class AuthInvalidLoginCredentialsException() : AuthException("Invalid login credentials.");
