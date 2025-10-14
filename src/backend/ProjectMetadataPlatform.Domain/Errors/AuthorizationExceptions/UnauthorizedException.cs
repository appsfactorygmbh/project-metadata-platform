namespace ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

/// <summary>
/// Exception thrown when access is unauthorized.
/// </summary>
public class UnauthorizedException()
    : AuthorizationException("User is unauthorized to access this ressource.");
