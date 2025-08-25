namespace ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

/// <summary>
/// Exception thrown when an entity already exists in the system.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public class EntityAlreadyExistsException(string message) : PmpException(message);
