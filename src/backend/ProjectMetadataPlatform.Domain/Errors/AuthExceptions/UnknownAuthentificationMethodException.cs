namespace ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

/// <summary>
/// Thrown when a request was authentified with an unknown method.
/// </summary>
public class UnknownAuthentificationMethodException()
    : AuthException("Unknown Authentification Method.");
