namespace ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

/// <summary>
/// Represents an abstract base class for authorization-related exceptions.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public abstract class AuthorizationException(string message) : PmpException(message);
