namespace ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

/// <summary>
/// Represents an exception that occurs when an entity is not found.
/// </summary>
public class EntityNotFoundException(string message) : PmpException(message);
