namespace ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

/// <summary>
/// Exception thrown when access is unauthorized.
/// </summary>
public class UnauthorizedException()
    : AuthorizationException("Actor is unauthorized to access this ressource.");
