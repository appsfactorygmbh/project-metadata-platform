namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

/// <summary>
/// Represents an abstract base class for plugin-related exceptions, used to mark exceptions that are related to plugins and need specific error responses.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public abstract class PluginException(string message) : PmpException(message);
