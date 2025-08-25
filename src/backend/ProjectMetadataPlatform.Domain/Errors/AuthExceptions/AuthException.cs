namespace ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

/// <summary>
/// Represents an abstract base class for authentication-related exceptions.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public abstract class AuthException(string message) : PmpException(message);
